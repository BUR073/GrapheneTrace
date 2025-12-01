using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var query = _context.SensorData
                .Include(sd => sd.Heatmap)
                .ThenInclude(h => h.Chunks)
                .ThenInclude(c => c.Metrics)
                .Where(sd => sd.UserId == userId);

            if (dataId.HasValue)
                return await query.FirstOrDefaultAsync(sd => sd.DataId == dataId.Value);

            return await query.OrderByDescending(sd => sd.Timestamp).FirstOrDefaultAsync();
        }

        public async Task<List<SensorData>> GetAllSensorDataAsync(int userId)
        {
            return await _context.SensorData
                .Where(sd => sd.UserId == userId)
                .OrderByDescending(sd => sd.Timestamp)
                .ToListAsync();
        }

        public List<List<List<int>>> BuildHeatmapGrids(SensorData sensorData)
        {
            var result = new List<List<List<int>>>();
            if (sensorData?.Heatmap == null) return result;

            foreach (var chunk in sensorData.Heatmap.Chunks.OrderBy(c => c.ChunkNumber))
            {
                if (string.IsNullOrEmpty(chunk.ChunkData)) continue;

                var grid = chunk.ChunkData
                    .Split('\n')
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Split(',').Select(int.Parse).ToList())
                    .ToList();

                result.Add(grid);
            }

            return result;
        }
    }
}
