using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using GrapheneTrace.Data;
using GrapheneTrace.Models.Database; 
using GrapheneTrace.Services;

namespace GrapheneTrace.Services
{


    public class HeatmapService : IHeatmapService
    {
        
        private readonly ApplicationDbContext _context;
        
        public HeatmapService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CalculateMetrics(IEnumerable<string> chunkLines, int chunkId)
        {
            float peakPressure = CalculatePeakPressure(chunkLines);
            float contactAreaPercent =  CalculateContactAreaPercent(chunkLines);
            
            ChunkMetrics metrics = new ChunkMetrics()
            {
                ChunkId = chunkId,
                PeakPressureIndex =  peakPressure, 
                ContactAreaPercent = contactAreaPercent,
            };
            
            await _context.ChunkMetrics.AddAsync(metrics);
            await _context.SaveChangesAsync();
        }

        private float CalculatePeakPressure(IEnumerable<string> chunkLines)
        {
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