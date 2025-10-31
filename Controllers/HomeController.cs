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


namespace GrapheneTrace.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHeatmapService _heatmapService;
        private readonly ISensorDataService _sensorDataService;
        public HomeController(ILogger<HomeController> logger, 
            UserManager<ApplicationUser> userManager, 
            ApplicationDbContext context, 
            IHeatmapService heatmapService,
            ISensorDataService sensorDataService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _heatmapService = heatmapService; 
            _sensorDataService = sensorDataService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null) {
                return Challenge();
            }
            
            if (await _userManager.IsInRoleAsync(user, "Admin")) {
                return RedirectToAction("AdminHome");
            }
            
            if (await _userManager.IsInRoleAsync(user, "Clinician")) {
                return RedirectToAction("ClinicianHome");
            }
        
            return RedirectToAction("PatientHome");
            
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminHome(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            
            var userViewModelList = await _userManager.Users
                .Select(user => new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    Name = user.Name ?? string.Empty,
                    DateOfBirth = user.DateOfBirth,
                    
                    Roles = (from ur in _context.UserRoles
                             join r in _context.Roles on ur.RoleId equals r.Id
                             where ur.UserId == user.Id
                             select r.Name).ToList(),
                             
                    PatientLinkCount = _context.PatientClinician
                        .Count(pc => pc.PatientId == user.Id),
                    
                    ClinicianLinkCount = _context.PatientClinician
                        .Count(pc => pc.ClinicianId == user.Id)
                })
                .ToListAsync();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                var upperSearchString = searchString.ToUpper();

                userViewModelList = userViewModelList.Where(u => 
                    u.Email.ToUpper().Contains(upperSearchString) ||
                    u.Id.ToString() == searchString ||
                    (u.Name != null && u.Name.ToUpper().Contains(upperSearchString)) ||
                    u.Roles.Any(role => role.ToUpper().Contains(upperSearchString))
                ).ToList();
            }

            var viewModel = new AdminHomeViewModel
            {
                Users = userViewModelList
            };

            return View(viewModel);
        }
    

        [Authorize(Roles = "Clinician")]
        public IActionResult ClinicianHome()
        {
            return View();
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> PatientHome(int? dataId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var recentSensorData = await _sensorDataService.GetRecentSensorDataAsync(user.Id, dataId);
            await _heatmapService.ProcessMissingMetricsAsync(recentSensorData);

            var viewModel = new PatientHomeViewModel
            {
                AllHeatmapGrids = _sensorDataService.BuildHeatmapGrids(recentSensorData),
                HeatmapTimestamp = recentSensorData?.Timestamp,
                AllSensorData = await _sensorDataService.GetAllSensorDataAsync(user.Id)
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