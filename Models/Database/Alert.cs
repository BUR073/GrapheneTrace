using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{

    public class Alert
    {
        [Key]
        public int AlertID { get; set; }
        
        public int DataId { get; set; }
        [ForeignKey(nameof(DataId))]
        public Data DataId { get; set; } 
        
        public string Alert { get; set; } 
        
        public DateTime TimeStamp { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}

