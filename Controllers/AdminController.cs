using GrapheneTrace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
                // Add the user to the selected role
                await _userManager.AddToRoleAsync(user, model.SelectedRole);
                // Optionally, redirect to a page showing all users or back to the admin home
                return RedirectToAction("AdminHome", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // If we got this far, something failed, redisplay form with the roles populated
        model.Roles = await _roleManager.Roles.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Name
        }).ToListAsync();
        return View(model);
    }
}