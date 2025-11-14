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
                    chunk.ChunkId,
                    sensorData.DataId
                );
                newMetricsList.Add(metrics);
            }
            
            await _context.ChunkMetrics.AddRangeAsync(newMetricsList);
            await _context.SaveChangesAsync();
        }

        private ChunkMetrics CalculateMetrics(IEnumerable<string> chunkLines, int chunkId, int DataId)
        {
            float peakPressure = CalculatePeakPressure(chunkLines);
            float contactAreaPercent =  CalculateContactAreaPercent(chunkLines);
            List<float> AverageMinMax = CalculateAverageAndMinMaxPressure(chunkLines, chunkId, DataId);
            float averagePressure = AverageMinMax[0];
            float minPressure = AverageMinMax[1];
            float maxPressure = AverageMinMax[2];
            
            ChunkMetrics metrics = new ChunkMetrics()
            {
                ChunkId = chunkId,
                PeakPressureIndex =  peakPressure, 
                ContactAreaPercent = contactAreaPercent,
                AveragePressure =  averagePressure,
                MinPressure = minPressure,
                MaxPressure = maxPressure
            };

            return metrics; 
        }

        private async Task createAlert(int value, int chunkId, int DataId)
        { 
            Alert alert = new Alert()
            {
                DataId = DataId, 
                AlertText = $"Abnormally high pressure: {value} ChunkId: {chunkId}";
                TimeStamp = DateTime.UtcNow,
                Updateat = DateTime.UtcNow, 
            }
                
            await _context.ChunkMetrics.AddRangeAsync(alert);
            await _context.SaveChangesAsync();
        }
        

        private List<float> CalculateAverageAndMinMaxPressure(IEnumerable<string> chunkLines, int chunkId, int DataId)
        {
            float totalVals = 1024;
            float total = 0;
            float minVal = 256;
            float maxVal = 0;

            foreach (var line in chunkLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var lineSplit = line.Split(',');

                foreach (var val in lineSplit)
                {
                    if (int.TryParse(val, out var value))
                    {
                        total +=  value;
                        if (value > 255)
                        {
                            createAlert(value, chunkId, DataId);
                        }

                        if (value > maxVal)
                        {
                            maxVal = value;
                        } else if (value < minVal && value != 0)
                        {
                            minVal = value;
                        }
                    }
                }
            }

            if (total == 0)
            {
                return new List<float> { 0.0f, 0.0f, 0.0f }; 
            }
            
            var average = (total / totalVals);
            return new List<float> { average, minVal, maxVal };

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