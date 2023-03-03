using System.ComponentModel.DataAnnotations;

namespace smarthomeAPI.Models
{
    public class UserPasswordResetRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required, MinLength(6, ErrorMessage = "Password must be atleast 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required, Compare("Password", ErrorMessage = "Passwords does not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
