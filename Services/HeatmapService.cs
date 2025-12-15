// SID: 2408078
using GrapheneTrace.Data;
using GrapheneTrace.Models.Database;
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
            // If there is no chunk defined
            if (sensorData?.Heatmap?.Chunks == null)
            {
                return;
            }

            // Get all the chunks to process
            var chunksToProcess = sensorData.Heatmap.Chunks
                .Where(c => c.Metrics == null) 
                .ToList();

            // If there are none 
            if (chunksToProcess.Count == 0)
            {
                return; 
            }

            // Init new list
            var newMetricsList = new List<ChunkMetrics>();
            // Loop through chunks to process
            foreach (var chunk in chunksToProcess)
            {
                // Calc the metrics
                var metrics = CalculateMetrics(
                    chunk.ChunkData.Split('\n'), 
                    chunk.ChunkId,
                    sensorData.DataId
                );
                // Add to list
                newMetricsList.Add(metrics);
            }
            
            // Save to db
            await _context.ChunkMetrics.AddRangeAsync(newMetricsList);
            await _context.SaveChangesAsync();
        }

        private ChunkMetrics CalculateMetrics(IEnumerable<string> chunkLines, int chunkId, int DataId)
        {

            var averageMinMax = CalculateAverageAndMinMaxPressure(chunkLines, chunkId, DataId);
            
            // Create metrics object
            ChunkMetrics metrics = new ChunkMetrics()
            {
                ChunkId = chunkId,
                PeakPressureIndex =  CalculatePeakPressure(chunkLines), 
                ContactAreaPercent = CalculateContactAreaPercent(chunkLines),
                AveragePressure =  averageMinMax[0],
                MinPressure = averageMinMax[1],
                MaxPressure = averageMinMax[2]
            };

            return metrics; 
        }

        private void CreateAlert(int value, int chunkId, int DataId)
        {
            // Create new alert
            var alert = new Alert()
            {
                DataId = DataId,
                SensorData = null!, 
                AlertText = $"Abnormally high pressure: {value} ChunkId: {chunkId}",
                TimeStamp = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            // add to context
            _context.Alert.Add(alert); 
        }
        

        private List<float> CalculateAverageAndMinMaxPressure(IEnumerable<string> chunkLines, int chunkId, int DataId)
        {
            // Define consts and init vars
            const float totalVals = 1024;
            float total = 0;
            float minVal = 256;
            float maxVal = 0;

            // Loop through chunk
            foreach (var line in chunkLines)
            {
                // If line is empty
                if (string.IsNullOrWhiteSpace(line)) continue;

                // split line on comma
                var lineSplit = line.Split(',');

                // Loop through line 
                foreach (var val in lineSplit)
                {
                    // Parse to int if not an int already
                    if (!int.TryParse(val, out var value)) continue;
                    
                    // Add to total
                    total +=  value;
                    
                    // Create alert if pressure over 255
                    if (value > 255)
                    {
                        CreateAlert(value, chunkId, DataId);
                    }

                    // Update min/max cvals
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
            
            // calc avr
            var average = (total / totalVals);
            // return all vals
            return [average, minVal, maxVal];

        }

        private float CalculatePeakPressure(IEnumerable<string> chunkLines)
        {
            // TODO: Logic here
            return 0.0f;
        }

        private float CalculateContactAreaPercent(IEnumerable<string> chunkLines)
        {
            // Define consts and init vars
            const float totalVals = 1024;
            float nonZeroVals = 0;
            
            // Loop through lines
            foreach (var line in chunkLines)
            {
                // If empty
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                // Split on comma 
                var lineSplit = line.Split(',');
                // Loop through line
                foreach (var val in lineSplit)
                {
                    // Parse to int
                    if (!int.TryParse(val, out var value)) continue;
                    // Update nonZeroVals
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
            
            // Return contact area
            return (nonZeroVals / totalVals) * 100;
        }
    }
    
}