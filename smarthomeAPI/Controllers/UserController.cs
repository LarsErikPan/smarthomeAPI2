using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smarthomeAPI.Services.UserService;
using System.Security.Claims;

namespace smarthomeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService) 
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public Task<IActionResult> Register(UserRegisterRequest request)
        {
            return _userService.Register(request);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public Task<IActionResult> Login(UserLoginRequest request)
        {
            return _userService.Login(request);
        }

        [AllowAnonymous]
        [HttpPost("verify")]
        public Task<IActionResult> Verify(String Token)
        {
            return _userService.Verify(Token);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public Task<IActionResult> ForgotPassword(String Email)
        {
            return _userService.ForgotPassword(Email);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public Task<IActionResult> ResetPassword(UserPasswordResetRequest request)
        {
            return _userService.ResetPassword(request);
        }

        [HttpPost("Create-enviroment")]
        public Task<IActionResult> CreateEnviroment(EnviromentRegisterRequest request)
        {
            return _userService.CreateEnviroment(request);
        }

        [HttpPost("Delete-enviroment")]
        public Task<IActionResult> DeleteEnviroment(EnviromentDeleteRequest request)
        {
            return _userService.DeleteEnviroment(request);
        }

        [HttpPost("Delete-user")]
        public Task<IActionResult> DeleteUser(EnviromentRegisterRequest request)
        {
            return _userService.DeleteUser(request);
        }

        [HttpPost("upload-rawdatafile")]
        public IActionResult Upload([FromForm] RawDataWriteRequest request)
        {
            return _userService.Upload(request);
        }

        [HttpGet("Get-all-enviroments")]
        public List<Enviroment> GetAllValues()
        {
            return _userService.GetAllValues();
        }
        [HttpPost("get-envToken")]
        public IActionResult GetEnvToken(string envPath)
        {
            return _userService.GetEnvToken(envPath);
        }
        [HttpGet("get-enviroments")]
        public List<String> GetEnvTokens()
        {
            return _userService.GetEnvTokens();
        }
    }
}
