// SID: 2408078
namespace GrapheneTrace.Models.Patient
{
    /// <summary>
    /// Model for storing chunk metrics
    /// </summary>
    public class ChunkMetricModel
    {
        public float PeakPressure { get; set; }
        public float ContactArea { get; set; }
        public float AveragePressure { get; set; }
        public float MaxPressure  { get; set; }
        public float MinPressure  { get; set; }
        
    }
}