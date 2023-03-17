using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace smarthomeAPI.Services.UserService
{
    public interface IUserService
    {
        Task<IActionResult> Register(UserRegisterRequest request);

        Task<IActionResult> Login(UserLoginRequest request);

        Task<IActionResult> Verify(String Token);

        Task<IActionResult> ForgotPassword(String Email);

        Task<IActionResult> ResetPassword(UserPasswordResetRequest request);

        Task<IActionResult> DeleteUser(EnvironmentRegisterRequest request);

        IActionResult Upload(RawDataWriteRequest request);

        string GetTestvalues();
    }
}
