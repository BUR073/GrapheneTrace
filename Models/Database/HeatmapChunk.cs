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
        public string Data { get; set; } = string.Empty; 
    }
}