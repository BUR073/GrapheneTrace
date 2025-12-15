// SID: 2408078
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
            // delete the feedbacl
            int? dataId = await _feedbackService.DeleteFeedback(feedbackId);

            // If dataId
            if (dataId != null)
            {
                // Return to patient home with data id
                return RedirectToAction(nameof(Pages.PatientHome), "Home", new { dataId });
            }
            // return to patient home
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
            // If the model is valid
            if (!ModelState.IsValid)
                return BadRequest();
            
            // Get the user
            var user = await _userManager.GetUserAsync(User);

            // If no user return to patient home
            if (user == null) return RedirectToAction(nameof(Pages.PatientHome), "Home");
            // Add the feedback
            var heatmap = await _feedbackService.AddFeedback(model, user.Id);

            // If heatmap found
            if (heatmap != null)
            {
                // return to patient home with heatap
                return RedirectToAction(nameof(Pages.PatientHome), "Home", new { dataId = heatmap });
            }

            // return to patient home
            return RedirectToAction(nameof(Pages.PatientHome), "Home");

        }
    }


}