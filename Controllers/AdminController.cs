// SID: 2408078
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
            // Populate CreateUserViewModel with roles
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
                // Call the admin service createUser func
                var result = await _adminService.CreateUser(model);
                
                // If user is created
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Pages.AdminHome), "Home");
                }
                // Loop through errors and add to model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            // Populate roles in model
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
            // Find user object
            var user = await _userManager.FindByIdAsync(id.ToString());
            // If user doesnt exist
            if (user == null)
            {
                return NotFound();
            }
            // get users role
            var userRoles = await _userManager.GetRolesAsync(user);
            // Get all user roles
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            // Create EditUserViewModel
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
            // Populate roles in model
            model.Roles = await _roleManager.Roles
                .Where(r => r.Name != null) 
                .Select(r => r.Name!)        
                .ToListAsync();
            
            // If the model state isn't valid
            if (!ModelState.IsValid)
                return View(model);
            
            // Get user id
            var currentAdminIdString = _userManager.GetUserId(User);

            // Parse user id to int
            if (int.TryParse(currentAdminIdString, out var currentAdminId))
            {
                // Make sure you dont remove your own admin role 
                if (model.Id == currentAdminId && !model.SelectedRoles.Contains("Admin"))
                {
                    ModelState.AddModelError(string.Empty, "Error: You cannot remove your own Administrator role.");
                    return View(model);
                }
            }
            
            // Update the user
            if (await _adminService.UpdateUser(model))
            {
                // return to admin home
                return RedirectToAction(nameof(Pages.AdminHome), "Home");
            }

            ModelState.AddModelError(string.Empty, "Failed to update user. Please check details and try again.");
            
            

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
            // Get user object
            var user = await _userManager.FindByIdAsync(id.ToString());
            // If user does not exist
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
            // Get user id
            var currentAdminIdString = _userManager.GetUserId(User);
            // parse user id to int
            int.TryParse(currentAdminIdString, out int currentAdminId);
            
            // Delete user
            var status = await _adminService.DeleteUserAsync(id, currentAdminId);
            
            // handle errors
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

            // Return to admin home
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
            // Get user
            var user = await _userManager.FindByIdAsync(id.ToString());
            // If user doesnt exist
            if (user == null) return NotFound();
            
            // Create manageLinksViewModel
            var model = new ManageLinksViewModel
            {
                PrimaryUserId = user.Id,
                PrimaryUserName = user.Name,
                PrimaryUserRole = userType,
                AssignedLinks = (await _adminService.GetLinkSelectionList(userType.Opposite(), id, LinkFilter.Assigned)).ToList(),
                AvailableLinks = (await _adminService.GetLinkSelectionList(userType.Opposite(), id, LinkFilter.Available)).ToList(),
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
            // Get already linked users
            var currentlyLinkedIds = await _adminService.GetAlreadyLinkedUsers(model.PrimaryUserId, model.PrimaryUserRole);

            // update the links
            await _adminService.UpdatePatientClinicianLinks(
                model.SelectedLinkIds.Except(currentlyLinkedIds).ToList(), 
                currentlyLinkedIds.Except(model.SelectedLinkIds).ToList(), 
                model.PrimaryUserId, 
                model.PrimaryUserRole);

            // Return to admin home
            return RedirectToAction(nameof(Pages.AdminHome), "Home");
        }

    }
}