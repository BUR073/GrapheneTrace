// SID: 2408078
using Microsoft.EntityFrameworkCore;
using GrapheneTrace.Data;
using GrapheneTrace.Models.Database;
using GrapheneTrace.Services.Interfaces;
using GrapheneTrace.Models.Patient;

namespace GrapheneTrace.Services
{
    public class SensorDataService : ISensorDataService
    {
        private readonly ApplicationDbContext _context;

        public SensorDataService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public List<ChunkMetricModel> GetMetrics(SensorData sensorData)
        {
            if (sensorData?.Heatmap?.Chunks == null)
                return [];

            // return all metrics 
            return sensorData.Heatmap.Chunks
                .OrderBy(c => c.ChunkNumber)
                .Select(c => new ChunkMetricModel
                {
                    PeakPressure = c.Metrics?.PeakPressureIndex ?? 0,
                    ContactArea = c.Metrics?.ContactAreaPercent ?? 0,
                    AveragePressure = c.Metrics?.AveragePressure ?? 0,
                    MaxPressure = c.Metrics?.MaxPressure ?? 0,
                    MinPressure = c.Metrics?.MinPressure ?? 0
                })
                .ToList();
        }

        public async Task<SensorData?> GetRecentSensorDataAsync(int userId, int? dataId)
        {
            // Get heatmap, chunks and metrics
            var query = _context.SensorData
                .Include(sd => sd.Heatmap)
                .ThenInclude(h => h.Chunks)
                .ThenInclude(c => c.Metrics)
                .Where(sd => sd.UserId == userId);

            // If dataId provided get by dataid
            if (dataId.HasValue)
                return await query.FirstOrDefaultAsync(sd => sd.DataId == dataId.Value);

            // return result
            return await query.OrderByDescending(sd => sd.Timestamp).FirstOrDefaultAsync();
        }

        public async Task<List<SensorData>> GetAllSensorDataAsync(int userId)
        {
            // Return all sensor data
            return await _context.SensorData
                .Where(sd => sd.UserId == userId)
                .OrderByDescending(sd => sd.Timestamp)
                .ToListAsync();
        }

        public List<List<List<int>>> BuildHeatmapGrids(SensorData sensorData)
        {
            // Init lists
            var result = new List<List<List<int>>>();
            // If no heatmpa
            if (sensorData?.Heatmap == null) return result;
            
            // Loop through chunks in order
            foreach (var chunk in sensorData.Heatmap.Chunks.OrderBy(c => c.ChunkNumber))
            {
                // If no chunk
                if (string.IsNullOrEmpty(chunk.ChunkData)) continue;

                // Split into grid
                var grid = chunk.ChunkData
                    .Split('\n')
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Split(',').Select(int.Parse).ToList())
                    .ToList();
                
                // add grid to list
                result.Add(grid);
            }

            return result;
        }
    }
}
