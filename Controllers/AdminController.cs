using GrapheneTrace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

[Authorize(Roles = "Admin")] // This entire controller is only for Admins
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: /Admin/CreateUser
    [HttpGet]
    public async Task<IActionResult> CreateUser()
    {
        var viewModel = new CreateUserViewModel
        {
            // Get all roles and create a SelectListItem for each
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
            var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
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
    
    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        var model = new EditUserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            Roles = allRoles,
            SelectedRoles = userRoles.ToList()
        };

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.UserName = model.Email;

            var userRoles = await _userManager.GetRolesAsync(user);

            // Add the new roles
            var result = await _userManager.AddToRolesAsync(user, model.SelectedRoles.Except(userRoles));
            if (!result.Succeeded)
            {
                // Handle errors
            }

            // Remove the old roles
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(model.SelectedRoles));
            if (!result.Succeeded)
            {
                // Handle errors
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction("AdminHome", "Home");
        }

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }


    [HttpPost, ActionName("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUserConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            // Safety check: prevent admin from deleting their own account
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                // Optionally, add a model error to inform the admin
                TempData["ErrorMessage"] = "Error: You cannot delete your own administrator account.";
                return RedirectToAction("AdminHome", "Home");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("AdminHome", "Home");
            }
            // Handle errors if deletion fails
        }

        return RedirectToAction("AdminHome", "Home");
    }
}