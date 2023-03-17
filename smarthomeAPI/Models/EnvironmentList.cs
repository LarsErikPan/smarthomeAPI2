using System.ComponentModel.DataAnnotations;

namespace smarthomeAPI.Models
{
    public class EnvironmentList
    {
        public int ParentEnvironmentID { get; set; }
        public string EnvironmentName { get; set; } = string.Empty;
        public int EnvironmentID { get; set; }
        public int Depth { get; set; }
    }
}
