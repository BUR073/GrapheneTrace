using GrapheneTrace.Areas.Identity.Data;
using GrapheneTrace.Data;
using GrapheneTrace.Models.Admin;
using GrapheneTrace.Models.Database;
using GrapheneTrace.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GrapheneTrace.Tests;

public class TestHelpers
{
    public static EditUserViewModel CreateEditUserViewModel(int id, string email, string name, string newPassword = "", string confirmPassword = "")
    {
        return new EditUserViewModel
        {
            Id = id,
            Email = email,
            Roles = [],
            SelectedRoles = [],
            NewPassword = newPassword,
            ConfirmPassword = confirmPassword,
            Name = name,
            DateOfBirth = new DateTime(2000, 1, 1),
            
        };
    }

    public static Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var mgr = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

        return mgr;
    }
    
    public static ApplicationDbContext GetNewDb(string dbname)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbname)
            .Options;
        return new ApplicationDbContext(options);
    }

    public static AdminService GetNewAdminService(ApplicationDbContext context, Mock<UserManager<ApplicationUser>>? userManager = null)
    {
        if (userManager != null)
        {
            return new AdminService(userManager.Object, context);
        }

        return new AdminService(MockUserManager().Object, context);
    }

    public static async Task SeedDashboardUsers(string dbName)
    {
        await using var context = GetNewDb(dbName);
        var users = new List<ApplicationUser>
        {
            new ApplicationUser { Id = 1, Name = "Alice Patient", Email = "alice@test.com", DateOfBirth = new DateTime(1990, 1, 1) },
            new ApplicationUser { Id = 2, Name = "Bob Clinician", Email = "bob@test.com", DateOfBirth = new DateTime(1990, 1, 1) },
            new ApplicationUser { Id = 3, Name = "Charlie Admin", Email = "charlie@test.com", DateOfBirth = new DateTime(1990, 1, 1) },
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
    public static async Task SeedLink(string dbName, int clinicianId, int patientId)
    {
        await using var context = GetNewDb(dbName);
        context.PatientClinician.Add(new PatientClinician 
        { 
            ClinicianId = clinicianId, 
            PatientId = patientId      
        });
        await context.SaveChangesAsync();
    }

    public static ApplicationUser CreateUser(int id, string name, string email)
    {
        return new ApplicationUser { Id = id, Name = name, Email = email, DateOfBirth = new DateTime(1990, 1, 1) };
    }
}