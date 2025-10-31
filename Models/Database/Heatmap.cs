using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{
    
    public class Heatmap
    {
        [Key]
        public int HeatmapId { get; set; }

        public int DataId { get; set; }
        [ForeignKey(nameof(DataId))]
        public SensorData SensorData { get; set; } = null!;

        public ICollection<HeatmapChunk> Chunks { get; set; } = new List<HeatmapChunk>();
    }
}
