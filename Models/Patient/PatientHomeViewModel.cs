using System;
using System.Collections.Generic;
using GrapheneTrace.Models.Database;
using GrapheneTrace.Models.Feedback;

namespace GrapheneTrace.Models.Patient
{
    public class PatientHomeViewModel
    {
        public List<SensorData> AllSensorData { get; set; } = new();
        public List<List<List<int>>> AllHeatmapGrids { get; set; } = new();
        public List<ChunkMetricModel> AllMetrics { get; set; } = new();
        public DateTime? HeatmapTimestamp { get; set; }
        public IEnumerable<GrapheneTrace.Models.Database.Feedback> AllFeedback { get; set; }

    }
}