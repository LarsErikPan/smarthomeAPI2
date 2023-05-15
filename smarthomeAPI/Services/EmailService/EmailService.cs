using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using smarthomeAPI.Services.EmailService;

namespace smarthomeAPI.Services.EmailService
{
    public class EmailService : ControllerBase, IEmailService
    {
        public EmailService() { }
        public IActionResult SendEmail(string email, string token)
        {
            var mail = new MimeMessage();
            mail.From.Add(MailboxAddress.Parse("smrthm8@gmail.com"));
            mail.To.Add(MailboxAddress.Parse(email));
            mail.Subject = "SHPIA Verification";
            mail.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "<h1>Verification link for SHPIA</h1><a href=\"http://51.174.84.85:5000/api/user/verify?Token=" + token + "\">Verify</a><h2>Test</h2>" };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp-relay.sendinblue.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("smrthm8@gmail.com", "6LyzjOMX9I8sfWmh");
            smtp.Send(mail);
            smtp.Disconnect(true);

            return Ok("Verification Email sent");
        }
    }
}
