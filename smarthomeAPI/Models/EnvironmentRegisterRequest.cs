using System.ComponentModel.DataAnnotations;

namespace smarthomeAPI.Models
{
    public class EnvironmentRegisterRequest
    {
        public int ParentEnvironmentId {get; set;}
        [Required]
        public string EnvironmentName { get; set; } = string.Empty;
    }
}
                