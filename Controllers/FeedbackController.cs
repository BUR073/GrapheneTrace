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
        
        private readonly IFeedbackService _feedbackService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackController(IFeedbackService feedbackService, UserManager<ApplicationUser> userManager)
        {
            _feedbackService = feedbackService;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> DeleteFeedback(int feedbackId)
        {
            int? dataId = await _feedbackService.DeleteFeedback(feedbackId);

            if (dataId != null)
            {
                return RedirectToAction("PatientHome", "Home", new { dataId });
            }
            
            return RedirectToAction("PatientHome", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedback(NewFeedbackModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            var user = await _userManager.GetUserAsync(User);
            
            int? heatmap = await _feedbackService.AddFeedback(model, user.Id);

            if (heatmap != null)
            {
                return RedirectToAction("PatientHome", "Home", new { dataId = heatmap });
            }
            return RedirectToAction("PatientHome", "Home");

        }
    }


}