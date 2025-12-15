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
            // Get feedback object 
            var feedback = await _context.Feedback
                .Include(f => f.HeatmapChunk)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);

            // Check feedback exists
            if (feedback == null)
            {
                return null;
            }
            
            // Get dataId
            var dataId = feedback.HeatmapChunk?.ChunkId;

            // Delete feedback and save changes to db
            _context.Feedback.Remove(feedback);
            await _context.SaveChangesAsync();
                
            return dataId;
            
        }

        public async Task<int?> AddFeedback(NewFeedbackModel model, int user)
        {
            // Find the chunk
            var chunk = await _context.HeatmapChunk.FindAsync(model.ChunkId);
            // If the chunk doesnt exist
            if (chunk == null)
                return null;

            // Create new feedback model
            var feedback = new Feedback
            {
                UserId = user,
                ChunkId = chunk.ChunkId,
                Comment = model.Text,
                TimeStamp = DateTime.UtcNow
            };
            
            // Add and save to db
            _context.Feedback.Add(feedback);
            await _context.SaveChangesAsync();
            
            return chunk.HeatmapId; 
        }

    }
}