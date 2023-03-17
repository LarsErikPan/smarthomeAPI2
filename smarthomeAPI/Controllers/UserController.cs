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

        [HttpDelete("users")]
        public Task<IActionResult> DeleteUser(EnvironmentRegisterRequest request)
        {
            return _userService.DeleteUser(request);
        }

        [HttpPost("upload-rawdatafile")]
        public IActionResult Upload([FromForm] RawDataWriteRequest request)
        {
            return _userService.Upload(request);
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public string GetTestvalues()
        {
            return _userService.GetTestvalues();
        }
    }
}
