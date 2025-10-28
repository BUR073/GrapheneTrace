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
        public required ApplicationUser User { get; set; } = null!;
        
        public int ChunkId { get; set; }
        [ForeignKey(nameof(ChunkId))]
        public required HeatmapChunk HeatmapChunk { get; set; } = null!;
        
        public required string Comment { get; set; }
        
        public required DateTime TimeStamp { get; set; }
        
        public ICollection<FeedbackReply> Replies { get; set; } = new List<FeedbackReply>();
        
    }
}


