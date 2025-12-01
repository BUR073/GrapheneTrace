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
                .Where(c => c.Metrics == null) 
                .ToList();

            if (chunksToProcess.Count == 0)
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
            var peakPressure = CalculatePeakPressure(chunkLines);
            var contactAreaPercent =  CalculateContactAreaPercent(chunkLines);
            var averageMinMax = CalculateAverageAndMinMaxPressure(chunkLines, chunkId, DataId);
            var averagePressure = averageMinMax[0];
            var minPressure = averageMinMax[1];
            var maxPressure = averageMinMax[2];
            
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

        private void CreateAlert(int value, int chunkId, int DataId)
        {
            var alert = new Alert()
            {
                DataId = DataId,
                SensorData = null!, 
                AlertText = $"Abnormally high pressure: {value} ChunkId: {chunkId}",
                TimeStamp = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _context.Alert.Add(alert); 
        }
        

        private List<float> CalculateAverageAndMinMaxPressure(IEnumerable<string> chunkLines, int chunkId, int DataId)
        {
            const float totalVals = 1024;
            float total = 0;
            float minVal = 256;
            float maxVal = 0;

            foreach (var line in chunkLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var lineSplit = line.Split(',');

                foreach (var val in lineSplit)
                {
                    if (!int.TryParse(val, out var value)) continue;
                    
                    total +=  value;
                    if (value > 255)
                    {
                        CreateAlert(value, chunkId, DataId);
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

            if (total == 0)
            {
                return [0.0f, 0.0f, 0.0f]; 
            }
            
            var average = (total / totalVals);
            return [average, minVal, maxVal];

        }

        private float CalculatePeakPressure(IEnumerable<string> chunkLines)
        {
            //TODO: the logic
            return 0.0f;
        }

        private float CalculateContactAreaPercent(IEnumerable<string> chunkLines)
        {
            const float totalVals = 1024;
            float nonZeroVals = 0;
            
            foreach (var line in chunkLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var lineSplit = line.Split(',');
                foreach (var val in lineSplit)
                {
                    if (!int.TryParse(val, out var value)) continue;
                    if (value > 0)
                    {
                        nonZeroVals += 1;
                    }
                }
            }
            
            if (nonZeroVals == 0)
            {
                return 0.0f;
            }
            
            var contactArea = (nonZeroVals / totalVals) * 100;
            return contactArea;
        }
    }
    
}