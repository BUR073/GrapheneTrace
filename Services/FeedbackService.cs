// SID: 2408078
using GrapheneTrace.Models.Feedback;
using GrapheneTrace.Data;
using GrapheneTrace.Models.Database; 
using GrapheneTrace.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GrapheneTrace.Areas.Identity.Data; 


namespace GrapheneTrace.Services
{


    public class FeedbackService : IFeedbackService
    { 
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public FeedbackService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int?> DeleteFeedback(int feedbackId)
        {
            var feedback = await _context.Feedback
                .Include(f => f.HeatmapChunk)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);

            if (feedback == null)
            {
                return null;
            }
            var dataId = feedback.HeatmapChunk?.ChunkId;

            _context.Feedback.Remove(feedback);
            await _context.SaveChangesAsync();
                
            return dataId;
            
        }

        public async Task<int?> AddFeedback(NewFeedbackModel model, int user)
        {
            
            var chunk = await _context.HeatmapChunk.FindAsync(model.ChunkId);
            if (chunk == null)
                return null;

            var feedback = new Feedback
            {
                UserId = user,
                ChunkId = chunk.ChunkId,
                Comment = model.Text,
                TimeStamp = DateTime.UtcNow
            };

            _context.Feedback.Add(feedback);
            await _context.SaveChangesAsync();
            
            return chunk.HeatmapId; 
        }

    }
}