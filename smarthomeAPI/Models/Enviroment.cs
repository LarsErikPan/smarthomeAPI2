using System.ComponentModel.DataAnnotations;

namespace smarthomeAPI.Models
{
    [Index(nameof(UserId), nameof(ParentEnviromentID))]
    public class Enviroment
    {
        [Key]
        public int EnviromentId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ParentEnviromentID { get; set; }
        [MaxLength(20)]
        public string EnviromentName { get; set; } = string.Empty;
        public List<RawData> RawDatas { get; set; }
    }
}
