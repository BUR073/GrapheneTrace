using Microsoft.AspNetCore.Identity;
using GrapheneTrace.Areas.Identity.Data;
using Microsoft.Extensions.Logging;

namespace GrapheneTrace.Data.Seeders
{


    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roleNames = { "Admin", "Clinician", "Patient" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }
        }

        public static async Task SeedPatientsAsync(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            var patientsToSeed = new[]
            {
                new {
                    Id = 470800247,
                    UserName = "Patient1@gmail.com",
                    Email = "Patient1@gmail.com",
                    Name = "Patient1",
                    DateOfBirth = new DateTime(1992, 3, 12),
                    Password = "patient1"
                },
                new {
                    Id = 1910926003,
                    UserName = "Patient2@gmail.com",
                    Email = "Patient2@gmail.com",
                    Name = "Patient2",
                    DateOfBirth = new DateTime(1995, 6, 20),
                    Password = "patient2"
                },
                new {
                    Id = 1413301878,
                    UserName = "Patient3@gmail.com",
                    Email = "Patient3@gmail.com",
                    Name = "Patient3",
                    DateOfBirth = new DateTime(2001, 1, 8),
                    Password = "patient3"
                },
                new {
                    Id = 3509601203,
                    UserName = "Patient4@gmail.com",
                    Email = "Patient4@gmail.com",
                    Name = "Patient4",
                    DateOfBirth = new DateTime(1976, 11, 5),
                    Password = "patient4"
                },
                new {
                    Id = 3725499180,
                    UserName = "Patient5@gmail.com",
                    Email = "Patient5@gmail.com",
                    Name = "Patient5",
                    DateOfBirth = new DateTime(165, 8, 5),
                    Password = "patient5"
                }
                
            };

            foreach (var patientData in patientsToSeed)
            {
                if (await userManager.FindByEmailAsync(patientData.Email) == null)
                {
                    logger.LogInformation($"Attempting to create Patient: {patientData.Name}");

                    ApplicationUser patient = new ApplicationUser
                    {
                        Id = patientData.Id, 
                        UserName = patientData.UserName,
                        Email = patientData.Email,
                        EmailConfirmed = true,
                        Name = patientData.Name,
                        DateOfBirth = patientData.DateOfBirth,
                    };

                    var result = await userManager.CreateAsync(patient, patientData.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(patient, "Patient");
                        logger.LogInformation($"Patient created: {patient.Name}");
                    }
                    else
                    {
                        logger.LogError($"Patient user creation failed for {patient.Name}.");
                        foreach (var error in result.Errors)
                        {
                            logger.LogError($"Error: {error.Code} - {error.Description}");
                        }
                    }
                }
            }
        }

        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            if (await userManager.FindByEmailAsync("admin@admin.com") == null)
            {
                logger.LogInformation("Attempting to create admin user.");

                ApplicationUser admin = new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    Name = "Admin",
                    DateOfBirth = new DateTime(1980, 1, 1),
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
}