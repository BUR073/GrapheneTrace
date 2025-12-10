//SID: 2408078
using GrapheneTrace.Areas.Identity.Data;
using GrapheneTrace.Data;
using GrapheneTrace.Models.Admin;
using GrapheneTrace.Models.Database;
using GrapheneTrace.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GrapheneTrace.Tests
{
    /// <summary>
    /// A class of static helper functions for application testing
    /// </summary>
    public class TestHelpers
    {
        /// <summary>
        /// Create's and returns an EditUserViewModel
        /// </summary>
        /// <param name="id"></param> The id you want
        /// <param name="email"></param> The email you want
        /// <param name="name"></param> The name you want
        /// <param name="newPassword"></param> Optional: The new password
        /// <param name="confirmPassword"></param> Optional: The password confirmation
        /// <returns>EditerUserViewModel</returns>
        public static EditUserViewModel CreateEditUserViewModel(int id, string email, string name,
            string newPassword = "", string confirmPassword = "")
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

        /// <summary>
        /// Mocks the user manager
        /// </summary>
        /// <returns>A mocked UserManager</returns>
        public static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            return mgr;
        }

        /// <summary>
        /// Creates a new instance of ApplicationDbContext
        /// </summary>
        /// <param name="dbname"></param> The name of the database 
        /// <returns>ApplicationDbContext</returns>
        public static ApplicationDbContext GetNewDb(string dbname)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbname)
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates a new AdminService
        /// </summary>
        /// <param name="context"></param> Your database context
        /// <param name="userManager"></param> Optional: Your mock UserManager
        /// <returns>AdminService</returns>
        public static AdminService GetNewAdminService(ApplicationDbContext context,
            Mock<UserManager<ApplicationUser>>? userManager = null)
        {
            if (userManager != null)
            {
                return new AdminService(userManager.Object, context);
            }

            return new AdminService(MockUserManager().Object, context);
        }

        /// <summary>
        /// Seeds user's for the Admin Dashboard test's 
        /// </summary>
        /// <param name="dbName"></param> Database name
        public static async Task SeedDashboardUsers(string dbName)
        {
            await using var context = GetNewDb(dbName);
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = 1, Name = "Alice Patient", Email = "alice@test.com", DateOfBirth = new DateTime(1990, 1, 1)
                },
                new ApplicationUser
                    { Id = 2, Name = "Bob Clinician", Email = "bob@test.com", DateOfBirth = new DateTime(1990, 1, 1) },
                new ApplicationUser
                {
                    Id = 3, Name = "Charlie Admin", Email = "charlie@test.com", DateOfBirth = new DateTime(1990, 1, 1)
                },
            };
            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a new PatientClinican Link
        /// </summary>
        /// <param name="dbName"></param> The database name
        /// <param name="clinicianId"></param> The clinician's id
        /// <param name="patientId"></param> The patient's id
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

        /// <summary>
        /// Create a new User
        /// </summary>
        /// <param name="id"></param> The UserId
        /// <param name="name"></param> The User name
        /// <param name="email"></param> The user email
        /// <returns>ApplicationUser</returns>
        public static ApplicationUser CreateUser(int id, string name, string email)
        {
            return new ApplicationUser { Id = id, Name = name, Email = email, DateOfBirth = new DateTime(1990, 1, 1) };
        }

        /// <summary>
        /// Create's a new CreateUserViewModel
        /// </summary>
        /// <param name="email"></param> The new email
        /// <param name="name"></param> The new name
        /// <param name="password"></param> The new password
        /// <param name="role"></param> The role
        /// <returns>CreateUserViewModel</returns>
        public static CreateUserViewModel NewCreateUserViewModel(string email, string name, string password, string role)
        {
            return new CreateUserViewModel
            {
                Email = email,
                Name = name,
                Password = password,
                DateOfBirth = DateTime.Now,
                SelectedRole = role
            };
        }
    }
}