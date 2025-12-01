using GrapheneTrace.Models;
using GrapheneTrace.Models.Admin; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using GrapheneTrace.Areas.Identity.Data; 
using GrapheneTrace.Data;
using GrapheneTrace.Models.Patient;
using GrapheneTrace.Services.Interfaces;
using GrapheneTrace.Enums;


namespace GrapheneTrace.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHeatmapService _heatmapService;
        private readonly ISensorDataService _sensorDataService;
        private readonly IAdminService _adminService;
        public HomeController(
            UserManager<ApplicationUser> userManager, 
            ApplicationDbContext context, 
            IHeatmapService heatmapService,
            ISensorDataService sensorDataService,
            IAdminService adminService)
        {
            _userManager = userManager;
            _context = context;
            _heatmapService = heatmapService; 
            _sensorDataService = sensorDataService;
            _adminService = adminService;
        }
        
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null) {
                return Challenge();
            }
            
            if (await _userManager.IsInRoleAsync(user, nameof(UserType.Admin))) {
                return RedirectToAction(nameof(Pages.AdminHome));
            }
            
            if (await _userManager.IsInRoleAsync(user, nameof(UserType.Clinician))) {
                return RedirectToAction(nameof(Pages.ClinicianHome));
            }
        
            return RedirectToAction(nameof(Pages.PatientHome));
            
        }
            

        [Authorize(Roles = nameof(UserType.Admin))]
        public async Task<IActionResult> AdminHome(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            
            var viewModel = new AdminHomeViewModel
            {
                Users = await _adminService.GetAdminDashboardUsersAsync(searchString)
            };

            return View(viewModel);
        }
    

        [Authorize(Roles = nameof(UserType.Clinician))]
        public IActionResult ClinicianHome()
        {
            return View();
        }

        [Authorize(Roles = nameof(UserType.Patient))]
        public async Task<IActionResult> PatientHome(int? dataId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var recentSensorData = await _sensorDataService.GetRecentSensorDataAsync(user.Id, dataId);
            await _heatmapService.ProcessMissingMetricsAsync(recentSensorData);
            
            var feedback = await _context.Feedback
                .Include(f => f.HeatmapChunk)
                .Include(f => f.Replies)
                .Where(f => f.UserId == user.Id)
                .OrderByDescending(f => f.TimeStamp)
                .ToListAsync();


            var viewModel = new PatientHomeViewModel
            {
                AllHeatmapGrids = _sensorDataService.BuildHeatmapGrids(recentSensorData),
                HeatmapTimestamp = recentSensorData?.Timestamp,
                AllMetrics = _sensorDataService.GetMetrics(recentSensorData),
                AllSensorData = await _sensorDataService.GetAllSensorDataAsync(user.Id),
                AllFeedback = feedback
            };

            return View(viewModel);
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