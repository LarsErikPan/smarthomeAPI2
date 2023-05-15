using System.ComponentModel.DataAnnotations;

namespace smarthomeAPI.Models
{
    [Index(nameof(UserId), nameof(ParentEnvironmentID))]
    public class EnvironmentType
    {
        [Key]
        public int EnvironmentId { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int ParentEnvironmentID { get; set; }
        [MaxLength(20)]
        public string EnvironmentName { get; set; } = string.Empty;
        public virtual ICollection<RawData> RawDatas { get; set; }
        public int Depth { get; set; }
    }
}
