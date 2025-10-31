using System.Collections.Generic;

namespace GrapheneTrace.Services
{
    public interface IHeatmapService
    {
        float CalculatePeakPressure(IEnumerable<string> chunkLines);
        
        float CalculateContactAreaPercent(IEnumerable<string> chunkLines);
    }
}