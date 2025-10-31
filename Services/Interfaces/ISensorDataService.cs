using System.Collections.Generic;
using GrapheneTrace.Models.Database;
namespace GrapheneTrace.Services.Interfaces
{

    public interface ISensorDataService
    {
        Task<SensorData?> GetRecentSensorDataAsync(int userId, int? dataId);
        Task<List<SensorData>> GetAllSensorDataAsync(int userId);
        List<List<List<int>>> BuildHeatmapGrids(SensorData? sensorData);
    }
}