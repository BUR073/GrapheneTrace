using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{

    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        
        public int UserId { get; set; } 
        [ForeignKey(nameof(UserId))] 
        public ApplicationUser User { get; set; } = null!;
        
        public int DataId { get; set; }
        [ForeignKey(nameof(DataId))]
        public Data DataId { get; set; } = null!;
        
        public string Comment { get; set; }
        
        public DateTime TimeStamp { get; set; }
        
    }
}

//UserID FK
// DataID FK
// Feedback ID PK
// Comment
// DateTime