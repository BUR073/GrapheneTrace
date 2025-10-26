// In Controllers/AdminController.cs
using GrapheneTrace.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using GrapheneTrace.Areas.Identity.Data;


[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    
    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
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
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            
            // Stop Admin from setting themself to a non-admin role
            var currentAdminIdString = _userManager.GetUserId(User);
            if (int.TryParse(currentAdminIdString, out int currentAdminId))
            {
                if (user.Id == currentAdminId && !model.SelectedRoles.Contains("Admin"))
                {
                    ModelState.AddModelError(string.Empty, "Error: You cannot remove your own administrator role.");
                    model.Roles = await _roleManager.Roles.Select(r => r.Name).Where(n => n != null).ToListAsync() as List<string>;
                    return View(model);
                }
            }
            
            // Update all but password
            user.Email = model.Email;
            user.UserName = model.Email;
            user.DateOfBirth = model.DateOfBirth;
            user.Name = model.Name;
            
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.AddToRolesAsync(user, model.SelectedRoles.Except(userRoles));
            await _userManager.RemoveFromRolesAsync(user, userRoles.Except(model.SelectedRoles));
            
            // If there is a password check that it has been confirmed properly, then update
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                await _userManager.RemovePasswordAsync(user);
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }
            await _userManager.UpdateAsync(user);
        
            return RedirectToAction("AdminHome", "Home");
        }

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
        if (user != null)
        {
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
        }

        return RedirectToAction("AdminHome", "Home");
    }
}