using GrapheneTrace.Models;
using GrapheneTrace.Models.Admin; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using GrapheneTrace.Areas.Identity.Data; 
using GrapheneTrace.Data;
using System.Collections.Generic; 
using System.Linq;
using GrapheneTrace.Models.Database;
using GrapheneTrace.Models.Patient;
using GrapheneTrace.Services;
using GrapheneTrace.Services.Interfaces;
using GrapheneTrace.Models.Feedback;

namespace GrapheneTrace.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedback(NewFeedbackModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var chunk = await _context.HeatmapChunk.FindAsync(model.ChunkId);
            if (chunk == null)
                return NotFound("Chunk not found");

            var feedback = new Feedback
            {
                UserId = user.Id,
                ChunkId = chunk.ChunkId,
                Comment = model.Text,
                TimeStamp = DateTime.UtcNow
            };

            _context.Feedback.Add(feedback);
            await _context.SaveChangesAsync();

            // Optionally: TempData for a success message
            TempData["FeedbackMessage"] = "Your feedback has been submitted successfully.";

            return RedirectToAction("PatientHome", "Home", new { dataId = chunk.HeatmapId });

        }
    }


}