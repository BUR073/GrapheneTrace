using GrapheneTrace.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using GrapheneTrace.Areas.Identity.Data;
using GrapheneTrace.Services.Interfaces;
using GrapheneTrace.Enums;

namespace GrapheneTrace.Controllers
{


    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IAdminService _adminService;


        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, IAdminService adminService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var viewModel = new CreateUserViewModel
            {
                Roles = await _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }).ToListAsync()
            };
            return View(viewModel);
        }

        // POST: /Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    DateOfBirth = model.DateOfBirth,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);

                    return RedirectToAction("AdminHome", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.Roles = await _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }).ToListAsync();

            return View(model);
        }

        // GET: /Admin/EditUser/{id}
        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                DateOfBirth = user.DateOfBirth,
                Name = user.Name,
                Roles = allRoles.Where(r => r != null).ToList() as List<string>,
                SelectedRoles = userRoles.ToList()
            };

            return View(model);
        }



        // POST: /Admin/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentAdminIdString = _userManager.GetUserId(User);

            if (int.TryParse(currentAdminIdString, out var currentAdminId))
            {
                if (model.Id == currentAdminId && !model.SelectedRoles.Contains("Admin"))
                {
                    ModelState.AddModelError(string.Empty, "Error: You cannot remove your own Administrator role.");
                    model.Roles = await _roleManager.Roles
                        .Where(r => r.Name != null) 
                        .Select(r => r.Name!)        
                        .ToListAsync();

                    return View(model);
                }
            }
            
            if (await _adminService.UpdateUser(model))
            {
                return RedirectToAction("AdminHome", "Home");
            }

            ModelState.AddModelError(string.Empty, "Failed to update user. Please check details and try again.");
            
            model.Roles = await _roleManager.Roles
                .Where(r => r.Name != null) 
                .Select(r => r.Name!)        
                .ToListAsync();

            return View(model);
        }


        // GET: /Admin/DeleteUser/{id}
        [HttpGet]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /Admin/DeleteUser
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) {
                return RedirectToAction("AdminHome", "Home");
            }
            var currentAdminIdString = _userManager.GetUserId(User);
            
            if (int.TryParse(currentAdminIdString, out int currentAdminId))
            {
                if (user.Id == currentAdminId)
                {
                    TempData["ErrorMessage"] = "Error: You cannot delete your own administrator account.";
                    return RedirectToAction("AdminHome", "Home");
                }
            }

            await _userManager.DeleteAsync(user);

            return RedirectToAction("AdminHome", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ManagePatient(int id)
        {
            var patient = await _userManager.FindByIdAsync(id.ToString());
            if (patient == null) return NotFound();

            var allClinicians = await _userManager.GetUsersInRoleAsync("Clinician");

            var alreadyLinkedClinicianIds = await _adminService.GetAlreadyLinkedUsers(id, UserType.Clinician);

            var model = new ManageLinksViewModel
            {
                PrimaryUserId = patient.Id,
                PrimaryUserName = patient.Name,
                PrimaryUserRole = "Patient",
                
                AssignedLinks = _adminService.GetLinkSelectionList(allClinicians, alreadyLinkedClinicianIds, LinkFilter.Assigned).ToList(),

                AvailableLinks = _adminService.GetLinkSelectionList(allClinicians, alreadyLinkedClinicianIds, LinkFilter.Available).ToList(),


                SelectedLinkIds = alreadyLinkedClinicianIds
            };
            return View("ManageLinks", model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageClinician(int id)
        {
            var clinician = await _userManager.FindByIdAsync(id.ToString());
            if (clinician == null) return NotFound();

            var allPatients = await _userManager.GetUsersInRoleAsync("Patient");

            var alreadyLinkedPatientIds = await _adminService.GetAlreadyLinkedUsers(id, UserType.Patient);

            var model = new ManageLinksViewModel
            {
                PrimaryUserId = clinician.Id,
                PrimaryUserName = clinician.Name,
                PrimaryUserRole = "Clinician",
                AssignedLinks = _adminService.GetLinkSelectionList(allPatients, alreadyLinkedPatientIds, LinkFilter.Assigned).ToList(),
                AvailableLinks = _adminService.GetLinkSelectionList(allPatients, alreadyLinkedPatientIds, LinkFilter.Available).ToList(),
                SelectedLinkIds = alreadyLinkedPatientIds
            };
            return View("ManageLinks", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagePatient(ManageLinksViewModel model)
        {
            await UpdatePatientClinicianLinksAsync(
                primaryUserId: model.PrimaryUserId,
                selectedLinkIds: model.SelectedLinkIds,
                isManagingPatient: true);

            return RedirectToAction("AdminHome", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageClinician(ManageLinksViewModel model)
        {
            await UpdatePatientClinicianLinksAsync(
                primaryUserId: model.PrimaryUserId,
                selectedLinkIds: model.SelectedLinkIds,
                isManagingPatient: false);

            return RedirectToAction("AdminHome", "Home");
        }
        
        private async Task UpdatePatientClinicianLinksAsync(int primaryUserId, List<int> selectedLinkIds, bool isManagingPatient)
        {
            var currentlyLinkedIds = await _adminService.GetAlreadyLinkedUsers(
                primaryUserId,
                isManagingPatient ? UserType.Clinician : UserType.Patient
            );

            var idsToAdd = selectedLinkIds.Except(currentlyLinkedIds).ToList();
            var idsToRemove = currentlyLinkedIds.Except(selectedLinkIds).ToList();
            await _adminService.UpdatePatientClinicianLinks(idsToAdd, idsToRemove, primaryUserId, isManagingPatient);
        }
    }
}