using GrapheneTrace.Models.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using GrapheneTrace.Data;
using GrapheneTrace.Models.Database; 
using GrapheneTrace.Services;
using GrapheneTrace.Models;
using GrapheneTrace.Services.Interfaces;



using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using GrapheneTrace.Areas.Identity.Data; 
using GrapheneTrace.Data;
using System.Collections.Generic; 



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

            if (feedback != null)
            {
                int? dataId = feedback.HeatmapChunk?.ChunkId;

                _context.Feedback.Remove(feedback);
                await _context.SaveChangesAsync();
                
                return dataId;

            }

            return null; 
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