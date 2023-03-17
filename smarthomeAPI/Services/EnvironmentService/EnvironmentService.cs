using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace smarthomeAPI.Services.EnvironmentService
{
    public class EnvironmentService : ControllerBase, IEnvironmentService
    {

        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EnvironmentService(DataContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        // TODO: make this not have to query the sql database "twice"(once to find the parent and one to find if this allready exists
        public async Task<IActionResult> CreateEnvironment(EnvironmentRegisterRequest request)
        {
            // check that the token has a user ID
            string? temp = GetClaim("UserId");
            if (temp == null)
            {
                return BadRequest("string is empty");
            }
            int userId = Int32.Parse(temp);

            int ParentEnvironmentID;
            int Depth;

            EnvironmentType? parent;
            //check that the parent environment exists
            if (request.ParentEnvironmentPath == null)
            {
                ParentEnvironmentID = 0;
                Depth = 0;
            }
            else
            {
                parent = getEnvFromPath(request.ParentEnvironmentPath, userId);
                if (parent == null)
                {
                    return BadRequest("parent environment not found");
                }
                ParentEnvironmentID = parent.EnvironmentId;
                Depth = parent.Depth + 1;
            }


            //check that the environment does not allready exist.
            if (_context.Environments.FirstOrDefault(e =>
            e.ParentEnvironmentID == ParentEnvironmentID &
            e.EnvironmentName == request.EnvironmentName &
            e.UserId == userId) is not null)
            {
                return BadRequest("Environment allready exists");
            };

            //Write to db
            var environment = new EnvironmentType
            {
                UserId = userId,
                ParentEnvironmentID = ParentEnvironmentID,
                EnvironmentName = request.EnvironmentName,
                Depth = Depth
            };

            _context.Environments.Add(environment);
            await _context.SaveChangesAsync();

            return Ok("Environment succesfully created");
        }

        public async Task<IActionResult> DeleteEnvironment(EnvironmentDeleteRequest request)
        {
            string? temp = GetClaim("UserId");
            if (temp == null)
            {
                return BadRequest("string is empty");
            }

            EnvironmentType? Environment = _context.Environments.FirstOrDefault(e => e.EnvironmentId == request.EnvironmentId && e.UserId == Int32.Parse(temp));
            if (Environment is null)
            {
                return BadRequest("Environment does not exist");
            }
            _context.Environments.Remove(Environment);

            await _context.SaveChangesAsync();

            return Ok("Environment succesfully deleted");
        }

        public IActionResult GetEnvToken(string envPath)
        {
            string? claim = GetClaim("UserId");
            if (claim == null)
            {
                return BadRequest("AcessDenied");
            }
            int userId = Int32.Parse(claim);

            EnvironmentType? tempEnv = getEnvFromPath(envPath, userId);
            if (tempEnv is null)
            {
                return BadRequest("path invalid");
            }
            int envId = tempEnv.EnvironmentId;
            string token = CreateEnvWriteToken(envId.ToString(), userId.ToString());
            return Ok(token);
        }


        public List<EnvironmentType> GetEnvStrings()
        {
            string? userId = GetClaim("UserId");
            if (userId == null)
            {
                return new List<EnvironmentType>();
            }
            int iUserId = Int32.Parse(userId);
            return _context.Environments.Where(e =>
            e.UserId == iUserId)
                .OrderBy(env =>
                env.ParentEnvironmentID).ToList();
        }

        public EnvironmentList[] GetAllEnvironments()
        {
            string? userId = GetClaim("UserId");
            if (userId == null)
            {
                return new EnvironmentList[0];
            }
            return _context.Environments.Where(e => e.UserId == int.Parse(userId))
                .OrderByDescending(e => e.Depth)
                .Select(e => new EnvironmentList
                {
                    EnvironmentID = e.EnvironmentId,
                    EnvironmentName = e.EnvironmentName,
                    ParentEnvironmentID = e.ParentEnvironmentID,
                    Depth = e.Depth
                }).ToArray();
        }

        public Device[] GetDevices()
        {
            string? userId = GetClaim("UserId");
            if (userId == null)
            {
                return new Device[0];
            }
            return _context.RawDatas.Where(e => e.UserID == int.Parse(userId))
                .Select(e => new Device()
                {
                    EnvironmentID = e.EnvironmentID,
                    DeviceName = e.DeviceName,
                }).Distinct().ToArray();
        }

        private string? GetClaim(string claimName)
        {
            if (_httpContextAccessor.HttpContext is null)
            {
                return null;
            }
            ClaimsIdentity? identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return null;
            }

            var claim = identity.FindFirst(claimName);
            if (claim == null)
            {
                return null;
            }
            return claim.Value;
        }

        private string CreateEnvWriteToken(string envId, string userId)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("EnvId", envId),
                new Claim("EnvUserId", userId)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        // TODO: get a more efficient sql query that does not have to make a separate query for each child environment:)
        private EnvironmentType? getEnvFromPath(string path, int userId)
        {
            string[] pathList = path.Split('/');
            int envId = 0;
            EnvironmentType? env = default;

            foreach (string name in pathList)
            {
                env = _context.Environments.FirstOrDefault(e =>
                e.EnvironmentName == name &&
                e.UserId == userId &&
                e.ParentEnvironmentID == envId);
                if (env == null)
                {
                    return null;
                }
                envId = env.EnvironmentId;
            }
            return env;
        }
    }
}
