using Azure.Core.GeoJson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Bcpg;
using smarthomeAPI.Migrations;
using smarthomeAPI.Models;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace smarthomeAPI.Services.UserService
{
    public class UserService : ControllerBase, IUserService
    {

        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(DataContext context,IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest("User allready exists.");
            }

            CreatePasswordHash(request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("user succesfully created");
        }

        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            if (user.VerifiedAt == null)
            {
                return BadRequest("User is not verified.");
            }

            string token = CreateToken(user);
            return Ok(token);
        }


        public async Task<IActionResult> Verify(String Token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == Token);
            if (user == null)
            {
                return BadRequest("Invalid token");
            }

            user.VerifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok("succesfully verified.");
        }

        public async Task<IActionResult> ForgotPassword(String Email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.UtcNow.AddDays(1);
            await _context.SaveChangesAsync();

            return Ok("Password reset token sent");
        }

        public async Task<IActionResult> ResetPassword(UserPasswordResetRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("User not found");
            }

            CreatePasswordHash(request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return Ok("Password reset");
        }

        public async Task<IActionResult> CreateEnviroment(EnviromentRegisterRequest request)
        {
            // check that the token has a user ID
            string temp = GetClaim("UserId");
            if (temp == string.Empty)
            {
                return BadRequest("string is empty");
            }

            int userId = Int32.Parse(temp);

            //check that the parent environment exists
            if  (!(request.ParentEnviromentID == 0) & _context.Enviroments.FirstOrDefault(e => e.EnviromentId == request.ParentEnviromentID && e.UserId == userId) is null)
            {
                return BadRequest("Parent enviroment does not exist");
            }

            //check that the environment does not allready exist.
            if (!(_context.Enviroments.FirstOrDefault(e => 
            e.ParentEnviromentID == request.ParentEnviromentID & 
            e.EnviromentName == request.EnviromentName &
            e.UserId == userId) is null))
            {
                return BadRequest("Enviroment allready exists");
            };

            var enviroment = new Enviroment
            {
                UserId = userId,
                ParentEnviromentID = request.ParentEnviromentID,
                EnviromentName = request.EnviromentName
            };

            _context.Enviroments.Add(enviroment);
            await _context.SaveChangesAsync();

            return Ok("Enviroment succesfully created");
        }

        public async Task<IActionResult> DeleteEnviroment(EnviromentDeleteRequest request)
        {

            string temp = GetClaim("UserId");
            if (temp == string.Empty)
            {
                return BadRequest("string is empty");
            }


            Enviroment? enviroment = _context.Enviroments.FirstOrDefault(e => e.EnviromentId == request.EnviromentId && e.UserId == Int32.Parse(temp));
            if (enviroment is null)
            {
                return BadRequest("Enviroment does not exist");
            }
            _context.Enviroments.Remove(enviroment);

            await _context.SaveChangesAsync();

            return Ok("Enviroment succesfully deleted");
        }

        public async Task<IActionResult> DeleteUser(EnviromentRegisterRequest request)
        {
            var enviroment = _context.Enviroments.First();
            _context.Enviroments.Remove(enviroment);

            await _context.SaveChangesAsync();

            return Ok("Enviroment succesfully deleted");
        }

        public List<Enviroment> GetAllValues()
        {
            using (var context = _context)
            {
                var Envs = context.Enviroments.ToList();
                return Envs;
            }
        }
        public IActionResult GetEnvToken(string envPath)
        {
            string claim = GetClaim("UserId");
            if (claim == string.Empty)
            {
                return BadRequest("AcessDenied");
            }
            int userId = Int32.Parse(claim);

            String[] pathList = envPath.Split('/');
            int envId = 0;

            foreach (string name in pathList)
            {
                Enviroment? temp = _context.Enviroments.FirstOrDefault(e => 
                e.EnviromentName == name && 
                e.UserId == userId && 
                e.ParentEnviromentID == envId);
                if (temp == null)
                {
                    return BadRequest("Environment does not exist");
                }
                envId = temp.EnviromentId;
            }
            string token = CreateEnvWriteToken(envId.ToString());
            return Ok(token);
        }


        public List<String> GetEnvTokens()
        {
            string userId = GetClaim("UserId");
            if (userId == string.Empty)
            {
                return new List<string>();
            }
            int iUserId = Int32.Parse(userId);
            return _context.Enviroments.Where(e => e.UserId == iUserId).Select(y=>y.EnviromentName).ToList();
        }

        public IActionResult Upload(RawDataWriteRequest request)
        {
            string envId = GetClaim("EnvId");
            if (envId == string.Empty)
            {
                return BadRequest("Access denied");
            }

            var file = request.CsvFile;
            var lines = ReadAsStringAsync(file).Result;
            if (lines.Count() == 0)
                return Ok();

            DataTable dt = new DataTable();
            AddColumnsToDataTable(dt);

            string[] linesList = lines.Split('\n');
            foreach (string line in linesList.SkipLast(1))
            {
                dt.Rows.Add();
                string[] cell = line.Split(",");
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";

                dt.Rows[dt.Rows.Count - 1][0] = Int32.Parse(envId);
                dt.Rows[dt.Rows.Count - 1][1] = DateTime.Now;
                dt.Rows[dt.Rows.Count - 1][2] = DateTime.Parse(cell[0]);
                dt.Rows[dt.Rows.Count - 1][3] = cell[1];
                dt.Rows[dt.Rows.Count - 1][4] = Convert.ToDouble(cell[2], provider);
                dt.Rows[dt.Rows.Count - 1][5] = Convert.ToDouble(cell[3], provider);
                dt.Rows[dt.Rows.Count - 1][6] = Convert.ToDouble(cell[4], provider);
                dt.Rows[dt.Rows.Count - 1][7] = Convert.ToDouble(cell[5], provider);
                dt.Rows[dt.Rows.Count - 1][8] = Convert.ToDouble(cell[6], provider);
                dt.Rows[dt.Rows.Count - 1][9] = Convert.ToDouble(cell[7], provider);
                dt.Rows[dt.Rows.Count - 1][10] = Convert.ToDouble(cell[8], provider);
                dt.Rows[dt.Rows.Count - 1][11] = Convert.ToDouble(cell[9], provider);
                dt.Rows[dt.Rows.Count - 1][12] = Convert.ToDouble(cell[10], provider);
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetSection("sqlPath").Value))
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
                {
                    bulkCopy.DestinationTableName = "dbo.RawDatas";

                    SqlBulkCopyColumnMapping EnviromentId =
                    new SqlBulkCopyColumnMapping("EnviromentId", "EnviromentID");
                    bulkCopy.ColumnMappings.Add(EnviromentId);

                    SqlBulkCopyColumnMapping LoggedTime =
                    new SqlBulkCopyColumnMapping("LoggedTime", "LoggedTime");
                    bulkCopy.ColumnMappings.Add(LoggedTime);

                    SqlBulkCopyColumnMapping UploadTime =
                    new SqlBulkCopyColumnMapping("UploadTime", "UploadTime");
                    bulkCopy.ColumnMappings.Add(UploadTime);

                    SqlBulkCopyColumnMapping DeviceName =
                    new SqlBulkCopyColumnMapping("DeviceName", "DeviceName");
                    bulkCopy.ColumnMappings.Add(DeviceName);

                    SqlBulkCopyColumnMapping accelerometer_x =
                    new SqlBulkCopyColumnMapping("accelerometer_x", "accelerometer_x");
                    bulkCopy.ColumnMappings.Add(accelerometer_x);

                    SqlBulkCopyColumnMapping accelerometer_y =
                    new SqlBulkCopyColumnMapping("accelerometer_y", "accelerometer_y");
                    bulkCopy.ColumnMappings.Add(accelerometer_y);

                    SqlBulkCopyColumnMapping accelerometer_z =
                    new SqlBulkCopyColumnMapping("accelerometer_z", "accelerometer_z");
                    bulkCopy.ColumnMappings.Add(accelerometer_z);

                    SqlBulkCopyColumnMapping gyroscope_x =
                    new SqlBulkCopyColumnMapping("gyroscope_x", "gyroscope_x");
                    bulkCopy.ColumnMappings.Add(gyroscope_x);

                    SqlBulkCopyColumnMapping gyroscope_y =
                    new SqlBulkCopyColumnMapping("gyroscope_y", "gyroscope_y");
                    bulkCopy.ColumnMappings.Add(gyroscope_y);

                    SqlBulkCopyColumnMapping gyroscope_z =
                    new SqlBulkCopyColumnMapping("gyroscope_z", "gyroscope_z");
                    bulkCopy.ColumnMappings.Add(gyroscope_z);

                    SqlBulkCopyColumnMapping compass_x =
                    new SqlBulkCopyColumnMapping("compass_x", "compass_x");
                    bulkCopy.ColumnMappings.Add(compass_x);

                    SqlBulkCopyColumnMapping compass_y =
                    new SqlBulkCopyColumnMapping("compass_y", "compass_y");
                    bulkCopy.ColumnMappings.Add(compass_y);

                    SqlBulkCopyColumnMapping compass_z =
                    new SqlBulkCopyColumnMapping("compass_z", "compass_z");
                    bulkCopy.ColumnMappings.Add(compass_z);


                    con.Open();
                    bulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }
            return Ok("sucessfully written :)");

        }

        private void AddColumnsToDataTable(DataTable newData)
        {
            /*
            DataColumn Id = new DataColumn();
            Id.ColumnName = "Id";
            Id.DataType = System.Type.GetType("System.Int32");
            Id.AllowDBNull = false;
            Id.Unique = true;
            newData.Columns.Add(Id);
            */
            DataColumn EnviromentID = new DataColumn();
            EnviromentID.ColumnName = "EnviromentID";
            EnviromentID.DataType = System.Type.GetType("System.Int32");
            EnviromentID.AllowDBNull = true;
            EnviromentID.Unique = false;
            newData.Columns.Add(EnviromentID);

            DataColumn UploadTime = new DataColumn();
            UploadTime.ColumnName = "UploadTime";
            UploadTime.DataType = System.Type.GetType("System.DateTime");
            UploadTime.AllowDBNull = true;
            UploadTime.Unique = false;
            newData.Columns.Add(UploadTime);

            DataColumn LoggedTime = new DataColumn();
            LoggedTime.ColumnName = "LoggedTime";
            LoggedTime.DataType = System.Type.GetType("System.DateTime");
            LoggedTime.AllowDBNull = true;
            LoggedTime.Unique = false;
            newData.Columns.Add(LoggedTime); 
            
            DataColumn DeviceName = new DataColumn();
            DeviceName.ColumnName = "DeviceName";
            DeviceName.DataType = System.Type.GetType("System.String");
            DeviceName.AllowDBNull = true;
            DeviceName.Unique = false;
            newData.Columns.Add(DeviceName);

            DataColumn accelerometer_x = new DataColumn();
            accelerometer_x.ColumnName = "accelerometer_x";
            accelerometer_x.DataType = System.Type.GetType("System.Double");
            accelerometer_x.AllowDBNull = true;
            accelerometer_x.Unique = false;
            newData.Columns.Add(accelerometer_x);

            DataColumn accelerometer_y = new DataColumn();
            accelerometer_y.ColumnName = "accelerometer_y";
            accelerometer_y.DataType = System.Type.GetType("System.Double");
            accelerometer_y.AllowDBNull = true;
            accelerometer_y.Unique = false;
            newData.Columns.Add(accelerometer_y);

            DataColumn accelerometer_z = new DataColumn();
            accelerometer_z.ColumnName = "accelerometer_z";
            accelerometer_z.DataType = System.Type.GetType("System.Double");
            accelerometer_z.AllowDBNull = true;
            accelerometer_z.Unique = false;
            newData.Columns.Add(accelerometer_z);

            DataColumn gyroscope_x = new DataColumn();
            gyroscope_x.ColumnName = "gyroscope_x";
            gyroscope_x.DataType = System.Type.GetType("System.Double");
            gyroscope_x.AllowDBNull = true;
            gyroscope_x.Unique = false;
            newData.Columns.Add(gyroscope_x);

            DataColumn gyroscope_y = new DataColumn();
            gyroscope_y.ColumnName = "gyroscope_y";
            gyroscope_y.DataType = System.Type.GetType("System.Double");
            gyroscope_y.AllowDBNull = true;
            gyroscope_y.Unique = false;
            newData.Columns.Add(gyroscope_y);

            DataColumn gyroscope_z = new DataColumn();
            gyroscope_z.ColumnName = "gyroscope_z";
            gyroscope_z.DataType = System.Type.GetType("System.Double");
            gyroscope_z.AllowDBNull = true;
            gyroscope_z.Unique = false;
            newData.Columns.Add(gyroscope_z);

            DataColumn compass_x = new DataColumn();
            compass_x.ColumnName = "compass_x";
            compass_x.DataType = System.Type.GetType("System.Double");
            compass_x.AllowDBNull = true;
            compass_x.Unique = false;
            newData.Columns.Add(compass_x);

            DataColumn compass_y = new DataColumn();
            compass_y.ColumnName = "compass_y";
            compass_y.DataType = System.Type.GetType("System.Double");
            compass_y.AllowDBNull = true;
            compass_y.Unique = false;
            newData.Columns.Add(compass_y);

            DataColumn compass_z = new DataColumn();
            compass_z.ColumnName = "compass_z";
            compass_z.DataType = System.Type.GetType("System.Double");
            compass_z.AllowDBNull = true;
            compass_z.Unique = false;
            newData.Columns.Add(compass_z);

        }

        private static async Task<string> ReadAsStringAsync(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(await reader.ReadLineAsync());
            }
            return result.ToString();
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", user.UserId.ToString())
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

        private string CreateEnvWriteToken(string env)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("EnvId", env)
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

        private string GetClaim(string claimName)
        {
            if (_httpContextAccessor.HttpContext is null)
            {
                return string.Empty;
            }
            var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            var claim = identity.FindFirst(claimName);
            if (claim == null)
            {
                return string.Empty;
            }
            return claim.Value;
        }

    }
}
