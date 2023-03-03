using System.ComponentModel.DataAnnotations;

namespace smarthomeAPI.Models
{
    public class EnviromentRegisterRequest
    {
        public int ParentEnviromentID { get; set; }
        [Required]
        public string EnviromentName { get; set; } = string.Empty;
    }
}
                