using Microsoft.AspNetCore.Identity;
using GrapheneTrace.Areas.Identity.Data;
using Microsoft.Extensions.Logging;
using GrapheneTrace.Models.Database;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

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

        public static async Task SeedHeatmapDataAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger logger)
        {
            string baseDirectory = AppContext.BaseDirectory;
            string dataPath = Path.GetFullPath(Path.Combine(baseDirectory, "../../../Data/GrapheneTrace"));

            if (!Directory.Exists(dataPath))
            {
                logger.LogWarning($"Seed data directory not found: {dataPath}. Skipping heatmap seed.");
                return;
            }
            
            var emails = new List<string>
            {
                "Patient1@gmail.com", "Patient2@gmail.com", "Patient3@gmail.com", "Patient4@gmail.com",
                "Patient5@gmail.com"
            };
            
            foreach (var email in emails)
            {
                var user = await userManager.FindByEmailAsync(email);
                
                if (user == null)
                {
                    logger.LogWarning($"User {email} not found.");
                    continue;
                }
                
                string searchPattern = $"{user.Name}_*.csv";

                string[] userFiles = Directory.GetFiles(dataPath, searchPattern);
                
                foreach (var filePath in userFiles)
                {
                    try
                    {
                        string dateString = Path.GetFileNameWithoutExtension(filePath).Split('_').Last(); 
                        
                        DateTime fileTimestamp = DateTime.ParseExact(
                            dateString, 
                            "yyyyMMdd", 
                            CultureInfo.InvariantCulture, 
                            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                        
                        await ProcessFileAsync(context, user.Id, filePath, fileTimestamp, logger);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Failed to process file: {filePath}.");
                    }
                }
            }

        }

        private static async Task ProcessFileAsync(ApplicationDbContext context, int userId, string filePath,
            DateTime fileTimestamp, ILogger logger)
        {
            
            // Check wether the file has already been processed
            if (await context.SensorData.AnyAsync(s => s.UserId == userId && s.Timestamp == fileTimestamp))
            {
                logger.LogInformation($"Data for user {userId} on {fileTimestamp.ToShortDateString()} already exists.");
                return; 
            }
            
            logger.LogInformation($"Processing file: {filePath} for user {userId}");
            
            using var transaction = await context.Database.BeginTransactionAsync();
            // Try catch block to prevent loading half a file
            try
            {
                // Create the sensorData record
                var sensorData = new SensorData
                {
                    UserId = userId,
                    Timestamp = fileTimestamp 
                };
                await context.SensorData.AddAsync(sensorData);
                await context.SaveChangesAsync();
                
                // Create the heatmap record
                var heatmap = new Heatmap
                {
                    DataId = sensorData.DataId, 
                    PeakPressureIndex = 0.0f,   
                    ContactAreaPercent = 0.0f   
                };
                await context.Heatmap.AddAsync(heatmap);
                await context.SaveChangesAsync();
                
                int newHeatmapId = heatmap.HeatmapId;
                
                string[] allLines = await File.ReadAllLinesAsync(filePath);
                var dataLines = allLines.ToArray();
                
                // Define the chunk size
                const int chunkSize = 32;
                int chunkNumber = 0;
                
                // Init the list to store the chunks
                var chunks = new List<HeatmapChunk>();
                
                // Loop through all the chunks
                for (int i = 0; i < dataLines.Length; i += chunkSize)
                {
                    
                    var lines = dataLines.Skip(i).Take(chunkSize);
                    
                    // Join them all into one line seperated by new line chars - 1024 vals long
                    string chunkData = string.Join("\n", lines);
                    
                    // Create the new record
                    var chunk = new HeatmapChunk
                    {
                        HeatmapId = newHeatmapId, 
                        ChunkNumber = chunkNumber,
                        ChunkData = chunkData
                    };
                    
                    chunks.Add(chunk);

                    chunkNumber++;
                }
                
                await context.HeatmapChunk.AddRangeAsync(chunks);
                await context.SaveChangesAsync();

                // Now save all the new records
                await transaction.CommitAsync(); 
                logger.LogInformation($"Successfully processed {filePath}.");
            }
            catch (Exception ex)
            {
     
                await transaction.RollbackAsync(); 
                logger.LogError(ex, $"Failed to process file {filePath}. Transaction rolled back.");
            }
        }
        
        public static async Task SeedCliniciansAsync(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            var cliniciansToSeed = new[]
            {
                new {
                    UserName = "Clinician1@gmail.com",
                    Email = "Clinician1@gmail.com",
                    Name = "Dr. John Davis",
                    DateOfBirth = new DateTime(1992, 3, 12),
                    Password = "Clinician1"
                },
                new {
                    UserName = "Clinician2@gmail.com",
                    Email = "Clinician2@gmail.com",
                    Name = "Dr. Sarah Jenkins",
                    DateOfBirth = new DateTime(1995, 6, 20),
                    Password = "Clinician2"
                },
                new {
                    UserName = "Clinician3@gmail.com",
                    Email = "Clinician3@gmail.com",
                    Name = "Dr. Michael Chen",
                    DateOfBirth = new DateTime(2001, 1, 8),
                    Password = "Clinician3"
                },
                new {
                    UserName = "Clinician4@gmail.com",
                    Email = "Clinician4@gmail.com",
                    Name = "Dr. Emily Rodriguez",
                    DateOfBirth = new DateTime(1976, 11, 5),
                    Password = "Clinician4"
                },
                new {
                    UserName = "Clinician5@gmail.com",
                    Email = "Clinician5@gmail.com",
                    Name = "Dr. David Patel",
                    DateOfBirth = new DateTime(1965, 8, 5),
                    Password = "Clinician5"
                }
            };

            foreach (var clinicianData in cliniciansToSeed)
            {
                if (await userManager.FindByEmailAsync(clinicianData.Email) == null)
                {
                    logger.LogInformation($"Attempting to create Clinician: {clinicianData.Name}");

                    ApplicationUser clinician = new ApplicationUser
                    {
                        UserName = clinicianData.UserName,
                        Email = clinicianData.Email,
                        EmailConfirmed = true,
                        Name = clinicianData.Name,
                        DateOfBirth = clinicianData.DateOfBirth,
                    };

                    var result = await userManager.CreateAsync(clinician, clinicianData.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(clinican, "Clinician");
                        logger.LogInformation($"Clinican created: {clinician.Name}");
                    }
                    else
                    {
                        logger.LogError($"Clinican user creation failed for {clinician.Name}.");
                        foreach (var error in result.Errors)
                        {
                            logger.LogError($"Error: {error.Code} - {error.Description}");
                        }
                    }
                }
            }
        }
        public static async Task SeedPatientsAsync(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            var patientsToSeed = new[]
            {
                new {
                    UserName = "Patient1@gmail.com",
                    Email = "Patient1@gmail.com",
                    Name = "Alice Smith",
                    DateOfBirth = new DateTime(1992, 3, 12),
                    Password = "patient1"
                },
                new {
                    UserName = "Patient2@gmail.com",
                    Email = "Patient2@gmail.com",
                    Name = "Ben Williams",
                    DateOfBirth = new DateTime(1995, 6, 20),
                    Password = "patient2"
                },
                new {
                    UserName = "Patient3@gmail.com",
                    Email = "Patient3@gmail.com",
                    Name = "Chloe Brown",
                    DateOfBirth = new DateTime(2001, 1, 8),
                    Password = "patient3"
                },
                new {
                    UserName = "Patient4@gmail.com",
                    Email = "Patient4@gmail.com",
                    Name = "James Taylor",
                    DateOfBirth = new DateTime(1976, 11, 5),
                    Password = "patient4"
                },
                new {
                    UserName = "Patient5@gmail.com",
                    Email = "Patient5@gmail.com",
                    Name = "Olivia Evans",
                    DateOfBirth = new DateTime(1965, 8, 5),
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