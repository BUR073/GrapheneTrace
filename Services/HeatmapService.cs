using System;
using System.Collections.Generic;
using System.Linq;
using GrapheneTrace.Services;
namespace GrapheneTrace.Services
{


    public class HeatmapService : IHeatmapService
    {
        public float CalculatePeakPressure(IEnumerable<string> chunkLines)
        {
            return 0.0f;
        }

        public float CalculateContactAreaPercent(IEnumerable<string> chunkLines)
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