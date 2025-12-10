// SID: 2408078
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{
    
    public class FeedbackReply
    {   
        [Key]
        public int feedbackReplyId { get; set; }
        
        public int FeedbackId { get; set; }
        [ForeignKey(nameof(FeedbackId))] 
        public required Feedback Feedback { get; set; }
        
        public int UserId { get; set; } 
        [ForeignKey(nameof(UserId))] 
        public ApplicationUser User { get; set; } = null!;
        
        public DateTime TimeStamp { get; set; }
        
    }
}

