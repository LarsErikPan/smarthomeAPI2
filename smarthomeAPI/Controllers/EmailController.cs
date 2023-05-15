using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MimeKit;
using System.Net;

namespace smarthomeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
       
        [HttpPost]
        public IActionResult SendEmail()
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("smrthm8@gmail.com"));
            email.To.Add(MailboxAddress.Parse("pokk@live.no"));
            email.Subject = "Test Email Subject";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "test" };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp-relay.sendinblue.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("smrthm8@gmail.com", "6LyzjOMX9I8sfWmh");
            smtp.Send(email);
            smtp.Disconnect(true);

            return Ok();

        }
    }
}
