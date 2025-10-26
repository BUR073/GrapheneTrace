using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{
    [Table("Alert", Schema = "GrapheneTrace")]
    public class Alert
    {
        [Key]
        public int AlertID { get; set; }
        
        public int DataId { get; set; }
        [ForeignKey(nameof(DataId))]
        public required Data Data { get; set; } = null!;
        
        public required string AlertText { get; set; } 
        
        public required DateTime TimeStamp { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}

