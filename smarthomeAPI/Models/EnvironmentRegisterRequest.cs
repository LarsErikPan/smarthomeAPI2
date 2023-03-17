using System.ComponentModel.DataAnnotations;

namespace smarthomeAPI.Models
{
    public class EnvironmentRegisterRequest
    {
        public string? ParentEnvironmentPath { get; set; }
        [Required]
        public string EnvironmentName { get; set; } = string.Empty;
    }
}
                