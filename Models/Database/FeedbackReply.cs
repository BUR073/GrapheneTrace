using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{

    public class FeedbackReply
    {   
        [Key]
        public int FeedbackReplyID { get; set; }
        
        public int FeedbackID { get; set; }
        [ForeignKey(nameof(FeedbackID))] 
        public Feedback FeedbackID { get; set; }
        
        public int UserId { get; set; } 
        [ForeignKey(nameof(UserId))] 
        public ApplicationUser User { get; set; } = null!;
        
        public DateTime TimeStamp { get; set; }
        
    }
}

