using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GrapheneTrace.Areas.Identity.Data;
using GrapheneTrace.Enums;
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
        
        /// <summary>
        /// Delete the feedback and return to patient home
        /// </summary>
        /// <param name="feedbackId"></param> The feedback to be deleted
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteFeedback(int feedbackId)
        {
            int? dataId = await _feedbackService.DeleteFeedback(feedbackId);

            if (dataId != null)
            {
                return RedirectToAction(nameof(Pages.PatientHome), "Home", new { dataId });
            }
            
            return RedirectToAction(nameof(Pages.PatientHome), "Home");
        }

        /// <summary>
        /// Add feedback and return to patient home
        /// </summary>
        /// <param name="model"></param> The details of the feedback to be added
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddFeedback(NewFeedbackModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            var user = await _userManager.GetUserAsync(User);
            
            var heatmap = await _feedbackService.AddFeedback(model, user.Id);

            if (heatmap != null)
            {
                return RedirectToAction(nameof(Pages.PatientHome), "Home", new { dataId = heatmap });
            }
            return RedirectToAction(nameof(Pages.PatientHome), "Home");

        }
    }


}