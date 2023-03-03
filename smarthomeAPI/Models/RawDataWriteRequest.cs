using System.ComponentModel.DataAnnotations;

namespace smarthomeAPI.Models
{
    public class RawDataWriteRequest
    {
        [Required]
        public IFormFile CsvFile { get; set; }

    }
}
