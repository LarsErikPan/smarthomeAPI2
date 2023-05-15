using Microsoft.AspNetCore.Mvc;

namespace smarthomeAPI.Services.EmailService
{
    public interface IEmailService
    {
        IActionResult SendEmail(string email, string token);
    }
}
