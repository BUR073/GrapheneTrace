using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{
    public class Alert
    {
        [Key]
        public int AlertId { get; set; }
        
        public int DataId { get; set; }
        [ForeignKey(nameof(DataId))]
        public required SensorData SensorData { get; set; } = null!;
        
        public required string AlertText { get; set; } 
        
        public required DateTime TimeStamp { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}


