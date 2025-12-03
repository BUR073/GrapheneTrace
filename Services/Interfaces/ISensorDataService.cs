using System.Collections.Generic;
using GrapheneTrace.Models.Database;
using GrapheneTrace.Models.Patient;
namespace GrapheneTrace.Services.Interfaces
{

    public interface ISensorDataService
    {
        List<ChunkMetricModel> GetMetrics(SensorData sensorData);
        Task<SensorData?> GetRecentSensorDataAsync(int userId, int? dataId);
        Task<List<SensorData>> GetAllSensorDataAsync(int userId);
        List<List<List<int>>> BuildHeatmapGrids(SensorData sensorData);
        
    }
}