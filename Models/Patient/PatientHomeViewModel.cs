using GrapheneTrace.Models.Database;

namespace GrapheneTrace.Models.Patient
{
    /// <summary>
    /// Model for passing the data to Patient Home View, contains all heatmap/sensordata and metrics
    /// </summary>
    public class PatientHomeViewModel
    {
        public List<SensorData> AllSensorData { get; set; } = new();
        public List<List<List<int>>> AllHeatmapGrids { get; set; } = new();
        public List<ChunkMetricModel> AllMetrics { get; set; } = new();
        public DateTime? HeatmapTimestamp { get; set; }
        public IEnumerable<GrapheneTrace.Models.Database.Feedback> AllFeedback { get; set; }

    }
}