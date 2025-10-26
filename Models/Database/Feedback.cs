using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{
    [Table("Feedback", Schema = "GrapheneTrace")]
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        
        public int UserId { get; set; } 
        [ForeignKey(nameof(UserId))] 
        public required ApplicationUser User { get; set; } = null!;
        
        public int DataId { get; set; }
        [ForeignKey(nameof(DataId))]
        public required Data Data { get; set; } = null!;
        
        public required string Comment { get; set; }
        
        public required DateTime TimeStamp { get; set; }
        
    }
}
