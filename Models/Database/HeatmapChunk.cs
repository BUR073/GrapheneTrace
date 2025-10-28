using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{
    public class HeatmapChunk
    {
        [Key]
        public int ChunkId { get; set; }

        public int HeatmapId { get; set; }
        [ForeignKey(nameof(HeatmapId))]
        public Heatmap Heatmap { get; set; } = null!;
        public int ChunkNumber { get; set; }   
        public string ChunkData { get; set; } = string.Empty; 
        
        public ICollection<Feedback> Feedback { get; set; } = new List<Feedback>();
    }
}