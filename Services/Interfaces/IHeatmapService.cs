using System.Collections.Generic;
using GrapheneTrace.Models.Database;

namespace GrapheneTrace.Services.Interfaces
{
    public interface IHeatmapService
    {
        Task ProcessMissingMetricsAsync(SensorData sensorData);
    }
}