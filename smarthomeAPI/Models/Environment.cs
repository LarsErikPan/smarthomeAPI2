using System.ComponentModel.DataAnnotations;

namespace smarthomeAPI.Models
{
    [Index(nameof(UserId), nameof(ParentEnvironmentID))]
    public class EnvironmentType
    {
        [Key]
        public int EnvironmentId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ParentEnvironmentID { get; set; }
        [MaxLength(20)]
        public string EnvironmentName { get; set; } = string.Empty;
        public List<RawData> RawDatas { get; set; }
        public int Depth { get; set; }
    }
}
