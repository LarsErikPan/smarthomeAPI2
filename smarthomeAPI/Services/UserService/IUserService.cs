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

        Task<IActionResult> CreateEnviroment(EnviromentRegisterRequest request);

        Task<IActionResult> DeleteEnviroment(EnviromentDeleteRequest request);

        Task<IActionResult> DeleteUser(EnviromentRegisterRequest request);

        List<Enviroment> GetAllValues();
        IActionResult Upload(RawDataWriteRequest request);

        IActionResult GetEnvToken(string envpath);

        List<String> GetEnvTokens();
    }
}
