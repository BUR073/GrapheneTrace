using GrapheneTrace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GrapheneTrace.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser<int>> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser<int>> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("AdminHome");
            }
            else if (await _userManager.IsInRoleAsync(user, "Clinician"))
            {
                return RedirectToAction("ClinicianHome");
            }
            else
            {
                return RedirectToAction("UserHome");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminHome()
        {
            var viewModel = new AdminHomeViewModel
            {
                Users = await _userManager.Users.ToListAsync()
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Clinician")]
        public IActionResult ClinicianHome()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult UserHome()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}