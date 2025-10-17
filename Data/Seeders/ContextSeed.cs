namespace GrapheneTrace.Data.Seeders;
using Microsoft.AspNetCore.Identity;

public static class ContextSeed
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
    {
        string[] roleNames = { "Admin", "Clinician", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }
    }

    public static async Task SeedAdminAsync(UserManager<IdentityUser<int>> userManager, ILogger logger)
    {
        // Check if the admin user exists
        if (await userManager.FindByEmailAsync("admin@admin.com") == null)
        {
            logger.LogInformation("Attempting to create admin user.");

            IdentityUser<int> admin = new IdentityUser<int>
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                EmailConfirmed = true
            };
            
            // Use a strong password
            var result = await userManager.CreateAsync(admin, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                logger.LogInformation("Admin user created successfully and assigned to Admin role.");
            }
            else
            {
                logger.LogError("Admin user creation failed.");
                foreach (var error in result.Errors)
                {
                    logger.LogError("Error: {Code} - {Description}", error.Code, error.Description);
                }
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists. Skipping creation.");
        }
    }
}