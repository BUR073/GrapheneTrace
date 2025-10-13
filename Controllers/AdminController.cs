// In Controllers/AdminController.cs
using GrapheneTrace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser<int>> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    
    public AdminController(UserManager<IdentityUser<int>> userManager, RoleManager<IdentityRole<int>> roleManager)
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
            var user = new IdentityUser<int>
            {
                UserName = model.Email,
                Email = model.Email,
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

            user.Email = model.Email;
            user.UserName = model.Email;

            var userRoles = await _userManager.GetRolesAsync(user);

            await _userManager.AddToRolesAsync(user, model.SelectedRoles.Except(userRoles));
            await _userManager.RemoveFromRolesAsync(user, userRoles.Except(model.SelectedRoles));
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