// SID: 2408078
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GrapheneTrace.Models;
using GrapheneTrace.Areas.Identity.Data;
using GrapheneTrace.Enums;

namespace GrapheneTrace.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {

            _signInManager = signInManager;
        }

        /// <summary>
        /// Show the login page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Process the login and redirect to appropriate homepage
        /// </summary>
        /// <param name="model"></param> The login details
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Check the model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // Call sign in func
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            
            // If signed in
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Pages.Index), "Home");
            }
            
            // else show error and return view
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }
        
        /// <summary>
        /// Log the user out
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Pages.Index), "Home");
        }
    }
}