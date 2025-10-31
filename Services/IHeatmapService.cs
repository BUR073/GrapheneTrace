using System.Collections.Generic;

namespace GrapheneTrace.Services
{
    public interface IHeatmapService
    {
        
        Task CalculateMetrics(IEnumerable<string> chunkLines, int chunkId);
    }
}