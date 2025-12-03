using GrapheneTrace.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using GrapheneTrace.Areas.Identity.Data;
using GrapheneTrace.Services.Interfaces;
using GrapheneTrace.Enums;
using GrapheneTrace.Enums.Extensions;

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

        /// <summary>
        /// Show create user view and populate roles dropdown
        /// </summary>
        /// <returns></returns>
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
        

        /// <summary>
        /// Process the new user and return to admin home
        /// </summary>
        /// <param name="model"></param> The details of the new user
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _adminService.CreateUser(model);
                
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Pages.AdminHome), "Home");
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

 
        /// <summary>
        /// Show the edit user view and populate form
        /// </summary>
        /// <param name="id"></param> The user to be edited
        /// <returns></returns>
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


  
        /// <summary>
        /// Process the edit user req and return to admin home
        /// </summary>
        /// <param name="model"></param> The changes to be made
        /// <returns></returns>
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
                return RedirectToAction(nameof(Pages.AdminHome), "Home");
            }

            ModelState.AddModelError(string.Empty, "Failed to update user. Please check details and try again.");
            
            model.Roles = await _roleManager.Roles
                .Where(r => r.Name != null) 
                .Select(r => r.Name!)        
                .ToListAsync();

            return View(model);
        }
        
        /// <summary>
        /// Show the delete user page
        /// </summary>
        /// <param name="id"></param> The user to be deleted
        /// <returns></returns>
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
 

        /// <summary>
        /// Process a delete user req and return to admin home
        /// </summary>
        /// <param name="id"></param> The user to be deleted
        /// <returns></returns>
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var currentAdminIdString = _userManager.GetUserId(User);
            
            int.TryParse(currentAdminIdString, out int currentAdminId);
            
            var status = await _adminService.DeleteUserAsync(id, currentAdminId);
            
            switch (status)
            {
                case DeleteUserStatus.CannotDeleteSelf:
                    TempData["ErrorMessage"] = "Error: You cannot delete your own administrator account.";
                    break;

                case DeleteUserStatus.DatabaseError:
                    TempData["ErrorMessage"] = "Error: Could not delete user due to a database error.";
                    break;

                case DeleteUserStatus.UserNotFound:
                    TempData["ErrorMessage"] = "Error: User not found.";
                    break;

                case DeleteUserStatus.Success:
                    TempData["SuccessMessage"] = "User deleted successfully.";
                    break;
            }

            return RedirectToAction(nameof(Pages.AdminHome), "Home");
        }
        

        /// <summary>
        /// Show the manage user page and populate the assigned and available links 
        /// </summary>
        /// <param name="id"></param> The id of the user being managed
        /// <param name="userType"></param> The type of user being managed
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ManageUser(int id, UserType userType)
        {
            var patient = await _userManager.FindByIdAsync(id.ToString());
            if (patient == null) return NotFound();
            
            var model = new ManageLinksViewModel
            {
                PrimaryUserId = patient.Id,
                PrimaryUserName = patient.Name,
                PrimaryUserRole = userType,
                AssignedLinks = (await _adminService.GetLinkSelectionList(userType.Opposite(), id, LinkFilter.Assigned)).ToList(),
                AvailableLinks = (await _adminService.GetLinkSelectionList(userType.Opposite(), id, LinkFilter.Available)).ToList(),
                SelectedLinkIds = await _adminService.GetAlreadyLinkedUsers(id, userType)
            };
            return View(nameof(Pages.ManageLinks), model);
        }
        
        
        /// <summary>
        /// Process the manage user req and return to admin home
        /// </summary>
        /// <param name="model"></param> the details of the links to be added/removed
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUser(ManageLinksViewModel model)
        {
            var currentlyLinkedIds = await _adminService.GetAlreadyLinkedUsers(model.PrimaryUserId, model.PrimaryUserRole);

            await _adminService.UpdatePatientClinicianLinks(
                model.SelectedLinkIds.Except(currentlyLinkedIds).ToList(), 
                currentlyLinkedIds.Except(model.SelectedLinkIds).ToList(), 
                model.PrimaryUserId, 
                model.PrimaryUserRole);

            return RedirectToAction(nameof(Pages.AdminHome), "Home");
        }

    }
}