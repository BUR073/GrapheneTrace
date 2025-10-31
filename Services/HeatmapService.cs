using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using GrapheneTrace.Data;
using GrapheneTrace.Models.Database; 
using GrapheneTrace.Services;
using GrapheneTrace.Models;
using GrapheneTrace.Services.Interfaces;

namespace GrapheneTrace.Services
{


    public class HeatmapService : IHeatmapService
    {
        
        private readonly ApplicationDbContext _context;
        
        public HeatmapService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task ProcessMissingMetricsAsync(SensorData sensorData)
        {
            if (sensorData?.Heatmap?.Chunks == null)
            {
                return;
            }

            var chunksToProcess = sensorData.Heatmap.Chunks
                .Where(c => c.Metrics == null) // Find chunks without metrics
                .ToList();

            if (!chunksToProcess.Any())
            {
                return; 
            }

            var newMetricsList = new List<ChunkMetrics>();
            foreach (var chunk in chunksToProcess)
            {
                var metrics = CalculateMetrics(
                    chunk.ChunkData.Split('\n'), 
                    chunk.ChunkId
                );
                newMetricsList.Add(metrics);
            }
            
            await _context.ChunkMetrics.AddRangeAsync(newMetricsList);
            await _context.SaveChangesAsync();
        }

        private ChunkMetrics CalculateMetrics(IEnumerable<string> chunkLines, int chunkId)
        {
            float peakPressure = CalculatePeakPressure(chunkLines);
            float contactAreaPercent =  CalculateContactAreaPercent(chunkLines);
            
            ChunkMetrics metrics = new ChunkMetrics()
            {
                ChunkId = chunkId,
                PeakPressureIndex =  peakPressure, 
                ContactAreaPercent = contactAreaPercent,
            };

            return metrics; 
        }

        private float CalculatePeakPressure(IEnumerable<string> chunkLines)
        {
            //TODO: the logic
            return 0.0f;
        }

        private float CalculateContactAreaPercent(IEnumerable<string> chunkLines)
        {
            float totalVals = 1024;
            float NonZeroVals = 0;
            
            foreach (var line in chunkLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var lineSplit = line.Split(',');
                foreach (var val in lineSplit)
                {
                    if (int.TryParse(val, out int value))
                    {
                        if (value > 0)
                        {
                            NonZeroVals += 1;
                        }
                    }
                }
            }
            
            if (NonZeroVals == 0)
            {
                return 0.0f;
            }
            
            float contactArea = (NonZeroVals / totalVals) * 100;
            return contactArea;
        }
    }
}