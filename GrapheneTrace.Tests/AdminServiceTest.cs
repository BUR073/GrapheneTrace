using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using GrapheneTrace.Services;
using GrapheneTrace.Areas.Identity.Data; 
using GrapheneTrace.Models.Admin;        
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace GrapheneTrace.Tests
{

    public class AdminServiceTest
    {
        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            return mgr;
        }

        [Fact]
        public async Task Update_User_Should_Return_False_When_User_Does_Not_Exist()
        {
            var userManager = MockUserManager();
            var adminService = new AdminService(userManager.Object, null!, null!);
            var model = new EditUserViewModel { Id = 999 }; 
            
            userManager.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((ApplicationUser?)null);

            var result = await adminService.UpdateUser(model);

            Assert.False(result);
        }

        [Fact]
        public async Task Update_User_Should_Update_Fields_And_Roles_When_User_Exists()
        {
            var mockUserManager = MockUserManager();
            var service = new AdminService(mockUserManager.Object, null!, null!);
            
            var model = new EditUserViewModel
            {
                Id = 1,
                Email = "new@test.com",
                Name = "New Name",
                DateOfBirth = new DateTime(2000, 1, 1),
                SelectedRoles = new List<string> { "Admin" } 
            };
            
            var existingUser = new ApplicationUser
            {
                Id = 1,
                Email = "old@test.com",
                Name = "Old Name",
                DateOfBirth = new DateTime(1990, 1, 1)
            };
            
            var currentRoles = new List<string> { "Clinician" };
            
            mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(existingUser);
 
            mockUserManager.Setup(x => x.GetRolesAsync(existingUser)).ReturnsAsync(currentRoles);

            mockUserManager.Setup(x => x.UpdateAsync(existingUser)).ReturnsAsync(IdentityResult.Success);

            mockUserManager.Setup(x => x.AddToRolesAsync(existingUser, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);
            
            mockUserManager.Setup(x => x.RemoveFromRolesAsync(existingUser, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);
            
            var result = await service.UpdateUser(model);
            
            Assert.True(result);
            
            Assert.Equal("new@test.com", existingUser.Email);
            Assert.Equal("New Name", existingUser.Name);

            mockUserManager.Verify(x => x.AddToRolesAsync(existingUser,
                It.Is<IEnumerable<string>>(r => r.Contains("Admin"))), Times.Once);
            
            mockUserManager.Verify(x => x.RemoveFromRolesAsync(existingUser,
                It.Is<IEnumerable<string>>(r => r.Contains("Clinician"))), Times.Once);
            
            mockUserManager.Verify(x => x.UpdateAsync(existingUser), Times.Once);
        }
        
        [Fact]
        public async Task Update_User_Should_Update_Password_When_New_Password_Provided()
        {
            var mockUserManager = MockUserManager();
            var service = new AdminService(mockUserManager.Object, null!, null!);

            var model = new EditUserViewModel
            {
                Id = 1,
                NewPassword = "SuperSecretPassword123!", 
                ConfirmPassword = "SuperSecretPassword123",
                SelectedRoles = new List<string>()
            };

            var existingUser = new ApplicationUser
            {
                Id = 1,
                Email = "old@test.com",
                Name = "Old Name",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(existingUser);
            mockUserManager.Setup(x => x.GetRolesAsync(existingUser)).ReturnsAsync(new List<string>());
            mockUserManager.Setup(x => x.UpdateAsync(existingUser)).ReturnsAsync(IdentityResult.Success);
            
            mockUserManager.Setup(x => x.RemovePasswordAsync(existingUser)).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.AddPasswordAsync(existingUser, "SuperSecretPassword123!"))
                .ReturnsAsync(IdentityResult.Success);
            
            var result = await service.UpdateUser(model);
            
            Assert.True(result);
            
            mockUserManager.Verify(x => x.RemovePasswordAsync(existingUser), Times.Once);
            mockUserManager.Verify(x => x.AddPasswordAsync(existingUser, "SuperSecretPassword123!"), Times.Once);
        }

        // --- TEST 4: Password Update Failure ---
        [Fact]
        public async Task UpdateUser_ShouldReturnFalse_WhenAddPasswordFails()
        {
            // Arrange
            var mockUserManager = MockUserManager();
            var service = new AdminService(mockUserManager.Object, null!, null!);

            var model = new EditUserViewModel
            {
                Id = 1,
                NewPassword = "BadPassword"
            };

            var existingUser = new ApplicationUser
            {
                Id = 1,
                Email = "old@test.com",
                Name = "Old Name",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(existingUser);
            mockUserManager.Setup(x => x.GetRolesAsync(existingUser)).ReturnsAsync(new List<string>());
            mockUserManager.Setup(x => x.RemovePasswordAsync(existingUser)).ReturnsAsync(IdentityResult.Success);

            // Fail Scenario: Password too weak
            mockUserManager.Setup(x => x.AddPasswordAsync(existingUser, "BadPassword"))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Too weak" }));

            // Act
            var result = await service.UpdateUser(model);

            // Assert
            Assert.False(result); // Should fail immediately

            // Verify UpdateAsync was NEVER called because we returned early
            mockUserManager.Verify(x => x.UpdateAsync(existingUser), Times.Never);
        }
    }
}    
