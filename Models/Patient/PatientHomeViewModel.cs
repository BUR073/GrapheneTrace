using System;
using System.Collections.Generic;
using GrapheneTrace.Models.Database;

namespace GrapheneTrace.Models.Patient
{
    public class PatientHomeViewModel
    {
        public List<SensorData> AllSensorData { get; set; } = new();
        public List<List<List<int>>> AllHeatmapGrids { get; set; } = new();
        public DateTime? HeatmapTimestamp { get; set; }
    }
}