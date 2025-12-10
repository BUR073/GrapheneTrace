// SID: 2408078
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
        public ApplicationUser User { get; set; }
        
        public int ChunkId { get; set; }
        [ForeignKey(nameof(ChunkId))]
        public HeatmapChunk HeatmapChunk { get; set; } 
        
        public required string Comment { get; set; }
        
        public required DateTime TimeStamp { get; set; }
        
        public ICollection<FeedbackReply> Replies { get; set; } = new List<FeedbackReply>();
        
    }
}


