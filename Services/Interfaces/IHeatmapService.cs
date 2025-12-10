// SID: 2408078
using GrapheneTrace.Models.Database;

namespace GrapheneTrace.Services.Interfaces
{
    public interface IHeatmapService
    {
        Task ProcessMissingMetricsAsync(SensorData sensorData);
    }
}