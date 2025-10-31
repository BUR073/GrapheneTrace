using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{
    public class ChunkMetrics
    {
        [Key]
        [ForeignKey(nameof(HeatmapChunk))]
        public int ChunkId { get; set; }
        public HeatmapChunk HeatmapChunk { get; set; }
        public float PeakPressureIndex { get; set; }
        public float ContactAreaPercent { get; set; }
        public float AveragePressure { get; set; }
        public float MaxPressure{ get; set; }
        public float MinPressure { get; set; }
    }
}