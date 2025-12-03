using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using GrapheneTrace.Services;
using GrapheneTrace.Areas.Identity.Data; 
using GrapheneTrace.Models.Admin;        
using GrapheneTrace.Enums;
using Microsoft.EntityFrameworkCore;
namespace GrapheneTrace.Tests
{
    public class AdminServiceTest
    {
        [Fact]
        public async Task DeleteUserAsync_Should_Return_Success_When_User_Is_Deleted_Successfully()
        {
            await using var context = TestHelpers.GetNewDb(Guid.NewGuid().ToString());
            var mockUserManager = TestHelpers.MockUserManager();
            var service = TestHelpers.GetNewAdminService(context, mockUserManager);
            
            var targetUser = TestHelpers.CreateUser(10, "Joe Bloggs", "target@test.com");
            
            mockUserManager.Setup(x => x.FindByIdAsync("10"))
                .ReturnsAsync(targetUser);
            
            mockUserManager.Setup(x => x.DeleteAsync(targetUser))
                .ReturnsAsync(IdentityResult.Success);
            
            var result = await service.DeleteUserAsync(10, 1);
            
            Assert.Equal(DeleteUserStatus.Success, result);

            mockUserManager.Verify(x => x.DeleteAsync(targetUser), Times.Once);
        }
        [Fact]
        public async Task DeleteUserAsync_Should_Return_Database_Error_When_Delete_Fails()
        {

            await using var context = TestHelpers.GetNewDb(Guid.NewGuid().ToString());
            var mockUserManager = TestHelpers.MockUserManager();
            var service = TestHelpers.GetNewAdminService(context, mockUserManager);
            
            var targetUser = TestHelpers.CreateUser(10, "Dave Bloggs", "target@test.com");
            
            mockUserManager.Setup(x => x.FindByIdAsync("10"))
                .ReturnsAsync(targetUser);
            
            mockUserManager.Setup(x => x.DeleteAsync(targetUser))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "DB Error" }));
            
            var result = await service.DeleteUserAsync(10, 1);
            
            Assert.Equal(DeleteUserStatus.DatabaseError, result);
        }
        
        [Fact]
        public async Task DeleteUserAsync_Should_Return_User_Not_Found_When_User_Does_Not_Exist()
        {
            await using var context = TestHelpers.GetNewDb(Guid.NewGuid().ToString());
            var mockUserManager = TestHelpers.MockUserManager();
            var service = TestHelpers.GetNewAdminService(context, mockUserManager);
            
            mockUserManager.Setup(x => x.FindByIdAsync("99"))
                .ReturnsAsync((ApplicationUser)null!);
            
            var result = await service.DeleteUserAsync(99, 1);
            
            Assert.Equal(DeleteUserStatus.UserNotFound, result);
            
            mockUserManager.Verify(x => x.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }
        
        [Fact]
        public async Task DeleteUserAsync_Should_Return_Cannot_Delete_Self_When_You_Try_To_Delete_Yourself()
        {
            await using var context = TestHelpers.GetNewDb(Guid.NewGuid().ToString());
            var mockUserManager = TestHelpers.MockUserManager();
            var service = TestHelpers.GetNewAdminService(context, mockUserManager);

            var result = await service.DeleteUserAsync(1, 1);

            Assert.Equal(DeleteUserStatus.CannotDeleteSelf, result);
            
            mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
        }
        
        [Fact]
        public async Task CreateUser_Should_Delete_User_And_Return_Failure_When_Role_Assignment_Fails()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.GetNewDb(dbName);
            var mockUserManager = TestHelpers.MockUserManager();
            var roleError = new IdentityError { Description = "Role does not exist" };
            
            mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(roleError));
            
            mockUserManager
                .Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var service = TestHelpers.GetNewAdminService(context, mockUserManager);

            var model = TestHelpers.NewCreateUserViewModel("rolefail@example.com", "Dave Bloggs", "Password123!",
                "NotARealRole");
            
            var result = await service.CreateUser(model);
            
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description == "Role does not exist");
            
            mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            
            mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            
            mockUserManager.Verify(x => x.DeleteAsync(
                It.Is<ApplicationUser>(u => u.Email == model.Email)), Times.Once);
        }
        
        [Fact]
        public async Task CreateUser_Should_Return_Failure_When_User_Creation_Fails()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.GetNewDb(dbName);
            var mockUserManager = TestHelpers.MockUserManager();
            var identityError = new IdentityError { Description = "Password too weak" };
            
            mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            var service = TestHelpers.GetNewAdminService(context, mockUserManager);
            
            var model = TestHelpers.NewCreateUserViewModel("fail@example.com", "Dave Bloggs", "123", "Patient");
            
            var result = await service.CreateUser(model);

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description == "Password too weak");
            
            mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }
        
        [Fact]
        public async Task CreateUser_Should_Return_Success_When_User_And_Role_Are_Created_Successfully()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.GetNewDb(dbName);
            var mockUserManager = TestHelpers.MockUserManager();
            var service = TestHelpers.GetNewAdminService(context, mockUserManager);
            
            mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            var model = TestHelpers.NewCreateUserViewModel("test@example.com", "Test User", "Password123!", "Admin");
            
            var result = await service.CreateUser(model);
            
            Assert.True(result.Succeeded);
            
            mockUserManager.Verify(x => x.CreateAsync(
                It.Is<ApplicationUser>(u => u.Email == model.Email && u.Name == model.Name), 
                model.Password), Times.Once);
            
            mockUserManager.Verify(x => x.AddToRoleAsync(
                It.Is<ApplicationUser>(u => u.Email == model.Email), 
                model.SelectedRole), Times.Once);
            
            mockUserManager.Verify(x => x.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }
        [Fact]
        public async Task GetAdminDashboardUsersAsync_returns_nothing_when_search_string_doesnt_match_anything()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedDashboardUsers(dbName);

            await using var context = TestHelpers.GetNewDb(dbName);
            var service = TestHelpers.GetNewAdminService(context);
                
            var result = await service.GetAdminDashboardUsersAsync("Nothing");
                
            Assert.Empty(result);
        }
        [Fact]
        public async Task GetAdminDashboardUsersAsync_returns_correct_users_from_search_string()
        {
            var dbName = Guid.NewGuid().ToString();
            await  TestHelpers.SeedDashboardUsers(dbName);

            await using var context = TestHelpers.GetNewDb(dbName);
            var service = TestHelpers.GetNewAdminService(context);
                
            var result = await service.GetAdminDashboardUsersAsync("Alice");
                
            Assert.NotNull(result);
            Assert.Single(result); 
            var user = result.First();
            Assert.Equal("Alice Patient", user.Name);
            Assert.Equal("alice@test.com", user.Email);
        }
        [Fact]
        public async Task GetAdminDashboardUsersAsync_returns_all_users_when_search_is_empty()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedDashboardUsers(dbName);

            await using var context = TestHelpers.GetNewDb(dbName);
            var service = TestHelpers.GetNewAdminService(context);
                
            var result = await service.GetAdminDashboardUsersAsync("");
                
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetLinkSelectionList_returns_empty_when_no_users_found_in_role()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedLink(dbName, 99, 10);

            await using var context = TestHelpers.GetNewDb(dbName);
            var userManagerMock = TestHelpers.MockUserManager();
            var allPatients = new List<ApplicationUser> {};

            userManagerMock
                .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Patient)))
                .ReturnsAsync(allPatients);

            var service = TestHelpers.GetNewAdminService(context, userManagerMock);
                
            var result = await service.GetLinkSelectionList(UserType.Patient, 99, (LinkFilter)99);
                
            Assert.Empty(result);
        }
        [Fact]
        public async Task GetLinkSelectionList_returns_empty_for_invalid_filter_type()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedLink(dbName, 99, 10);
            
            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                var userManagerMock = TestHelpers.MockUserManager();
                var allPatients = new List<ApplicationUser>
                {
                    TestHelpers.CreateUser(10, "Linked Patient", "linked@test.com"),
                    TestHelpers.CreateUser(20, "Available Patient", "available@test.com")
                };

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Patient)))
                    .ReturnsAsync(allPatients);
                
                var service = new AdminService(userManagerMock.Object, context);
                
                var result = await service.GetLinkSelectionList(UserType.Patient, 99, (LinkFilter)99);
                
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task GetLinkSelectionList_returns_assigned_links_for_a_clinician()
        {
            var dbName = Guid.NewGuid().ToString();
            await  TestHelpers.SeedLink(dbName, 99, 10);

            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                var userManagerMock = TestHelpers.MockUserManager();
                var allPatients = new List<ApplicationUser>
                {
                    TestHelpers.CreateUser(10, "Linked Patient","linked@test.com"),
                    TestHelpers.CreateUser(20, "Available Patient", "available@test.com")
                };

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Patient)))
                    .ReturnsAsync(allPatients);
                
                var service = TestHelpers.GetNewAdminService(context, userManagerMock);
                
                var result = await service.GetLinkSelectionList(UserType.Patient, 99, LinkFilter.Assigned);
                
                Assert.Single(result);
                Assert.Equal("10", result.First().Value);
                Assert.Equal("Linked Patient (linked@test.com)", result.First().Text);
            }
        }
        
        [Fact]
        public async Task GetLinkSelectionList_returns_assigned_links_for_a_patient()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedLink(dbName, 10, 99);
            
            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                var userManagerMock = TestHelpers.MockUserManager();
                var allPatients = new List<ApplicationUser>
                {
                    TestHelpers.CreateUser(10, "Linked Clinician", "linked@test.com"),
                    TestHelpers.CreateUser(20, "Available Clinician", "available@test.com")
                };

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Clinician)))
                    .ReturnsAsync(allPatients);
                
                var service = TestHelpers.GetNewAdminService(context, userManagerMock);
                
                var result = await service.GetLinkSelectionList(UserType.Clinician, 99, LinkFilter.Assigned);
                
                Assert.Single(result);
                Assert.Equal("10", result.First().Value);
                Assert.Equal("Linked Clinician (linked@test.com)", result.First().Text);
            }
        }
    
        [Fact]
        public async Task GetLinkSelectionList_returns_available_links_for_a_patient()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedLink(dbName, 10, 99);
            
            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                var userManagerMock = TestHelpers.MockUserManager();
                var allPatients = new List<ApplicationUser>
                {
                    TestHelpers.CreateUser(10, "Linked Clinician", "linked@test.com"),
                    TestHelpers.CreateUser(20, "Available Clinician","available@test.com")
                };

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Clinician)))
                    .ReturnsAsync(allPatients);
                
                var service = TestHelpers.GetNewAdminService(context, userManagerMock);
                
                var result = await service.GetLinkSelectionList(UserType.Clinician, 99, LinkFilter.Available);
                
                Assert.Single(result);
                Assert.Equal("20", result.First().Value);
                Assert.Equal("Available Clinician (available@test.com)", result.First().Text);
            }
        }
        
        [Fact]
        public async Task GetLinkSelectionList_returns_available_links_for_a_clinician()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedLink(dbName, 99, 10);
            
            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                var userManagerMock = TestHelpers.MockUserManager();
                var allPatients = new List<ApplicationUser>
                {
                    TestHelpers.CreateUser(10, "Linked Patient", "linked@test.com"),
                    TestHelpers.CreateUser(20, "Available Patient", "available@test.com")
                };

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Patient)))
                    .ReturnsAsync(allPatients);
                
                var service = TestHelpers.GetNewAdminService(context, userManagerMock);
                
                var result = await service.GetLinkSelectionList(UserType.Patient, 99, LinkFilter.Available);
                
                Assert.Single(result);
                Assert.Equal("20", result.First().Value);
                Assert.Equal("Available Patient (available@test.com)", result.First().Text);
            }
        }

        [Fact]
        public async Task GetAlreadyLinkedUsers_should_return_linked_patients()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedLink(dbName, 1, 1);
            await TestHelpers.SeedLink(dbName, 1, 2);
            await TestHelpers.SeedLink(dbName, 2, 99);

            await using var context = TestHelpers.GetNewDb(dbName);
            var service = TestHelpers.GetNewAdminService(context);
            var result = await service.GetAlreadyLinkedUsers(1, UserType.Patient);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(1, result);
            Assert.Contains(2, result);
            Assert.DoesNotContain(99, result);
        }

        [Fact]
        public async Task GetAlreadyLinkedUsers_should_return_linked_clinicians()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedLink(dbName, 3, 1);
            await TestHelpers.SeedLink(dbName, 2, 1);
            await TestHelpers.SeedLink(dbName, 5, 99);

            await using var context = TestHelpers.GetNewDb(dbName);
            
            var service = TestHelpers.GetNewAdminService(context);
            var result = await service.GetAlreadyLinkedUsers(1, UserType.Clinician);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(3, result);
            Assert.Contains(2, result);
            Assert.DoesNotContain(5, result);
        }

        [Fact]
        public async Task UpdateLinks_Should_Work_For_Clinician_Managing_Patients()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedLink(dbName, 1, 200);
            
            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                var service = TestHelpers.GetNewAdminService(context); 
                await service.UpdatePatientClinicianLinks([100], [200], 1, UserType.Patient);
            }
            
            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                var links = await context.PatientClinician.ToListAsync();
                Assert.Single(links);
                Assert.Contains(links, l => l is { ClinicianId: 1, PatientId: 100 });
                Assert.DoesNotContain(links, l => l is { ClinicianId: 1, PatientId: 200 });
            }
        }
        
        [Fact]
        public async Task UpdateLinks_Should_Work_For_Patient_Managing_Clinicians()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedLink(dbName, 6, 50);
            
            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                var service = TestHelpers.GetNewAdminService(context);
                await service.UpdatePatientClinicianLinks([5], [6], 50, UserType.Clinician);
            }
            
            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                var links = await context.PatientClinician.ToListAsync();

                Assert.Single(links);
                Assert.Contains(links, l => l is { PatientId: 50, ClinicianId: 5 });
                Assert.DoesNotContain(links, l => l is { PatientId: 50, ClinicianId: 6 });
            }
        }
        
        [Fact]
        public async Task UpdateLinks_Should_Do_Nothing_If_Lists_Are_Empty()
        {
            var dbName = Guid.NewGuid().ToString();
            await TestHelpers.SeedLink(dbName, 1, 1);
            
            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                var service = new AdminService(null!, context);
                await service.UpdatePatientClinicianLinks([], [], 1, UserType.Patient);
            }
            
            await using (var context = TestHelpers.GetNewDb(dbName))
            {
                Assert.Equal(1, await context.PatientClinician.CountAsync());
            }
        }
        
        [Fact]
        public async Task Update_User_Should_Return_False_When_User_Does_Not_Exist()
        {
            var userManager = TestHelpers.MockUserManager();
            var adminService = new AdminService(userManager.Object, null!);
            var model = TestHelpers.CreateEditUserViewModel(999, "test@test.com", "Test User");
            
            userManager.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((ApplicationUser?)null);

            var result = await adminService.UpdateUser(model);

            Assert.False(result);
        }

        [Fact]
        public async Task Update_User_Should_Update_Fields_And_Roles()
        {
            var mockUserManager = TestHelpers.MockUserManager();
            var service = new AdminService(mockUserManager.Object,null!);
            var model = new EditUserViewModel
            {
                Id = 1,
                Email = "new@test.com",
                Name = "New Name",
                DateOfBirth = new DateTime(2000, 1, 1),
                SelectedRoles = ["Admin"]
            };

            var existingUser = TestHelpers.CreateUser(1, "old@test.com", "Old Name");

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
            var mockUserManager = TestHelpers.MockUserManager();
            var service = new AdminService(mockUserManager.Object,null!);
            var model = TestHelpers.CreateEditUserViewModel(1, "test@test.com", "Test User", "SuperSecretPassword123!", "SuperSecretPassword123!");
            var existingUser = TestHelpers.CreateUser(1, "test@test.com", "Test User");
            
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
        [Fact]
        public async Task Update_User_Should_Not_Update_Password_if_it_does_not_match_password_confirm()
        {
            var mockUserManager = TestHelpers.MockUserManager();
            var service = new AdminService(mockUserManager.Object, null!);
            var model = TestHelpers.CreateEditUserViewModel(1, "test@test.com", "Test User", "SuperSecretPassword123!", "SuperSecretPassword123");
            var existingUser = TestHelpers.CreateUser(1, "test@test.com", "Test User");
            
            mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(existingUser);
            mockUserManager.Setup(x => x.GetRolesAsync(existingUser)).ReturnsAsync(new List<string>());
            mockUserManager.Setup(x => x.UpdateAsync(existingUser)).ReturnsAsync(IdentityResult.Success);
            
            mockUserManager.Setup(x => x.RemovePasswordAsync(existingUser)).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.AddPasswordAsync(existingUser, "SuperSecretPassword123!"))
                .ReturnsAsync(IdentityResult.Success);
            
            var result = await service.UpdateUser(model);
            
            Assert.False(result);
            
            mockUserManager.Verify(x => x.RemovePasswordAsync(existingUser), Times.Never);
            mockUserManager.Verify(x => x.AddPasswordAsync(existingUser, "SuperSecretPassword123!"), Times.Never);
        }
        
        [Fact]
        public async Task UpdateUser_Should_Return_false_when_password_to_weak()
        {
            var mockUserManager = TestHelpers.MockUserManager();
            var service = new AdminService(mockUserManager.Object, null!);
            var model = TestHelpers.CreateEditUserViewModel(1, "test@test.com", "Test User", "BadPassword", "BadPassword");
            
            var existingUser = TestHelpers.CreateUser(1, "old@test.com", "Old Name");

            mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(existingUser);
            mockUserManager.Setup(x => x.GetRolesAsync(existingUser)).ReturnsAsync(new List<string>());
            mockUserManager.Setup(x => x.RemovePasswordAsync(existingUser)).ReturnsAsync(IdentityResult.Success);
            
            mockUserManager.Setup(x => x.AddPasswordAsync(existingUser, "BadPassword"))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Too weak" }));
            
            var result = await service.UpdateUser(model);
            Assert.False(result); 
            mockUserManager.Verify(x => x.UpdateAsync(existingUser), Times.Never);
        }
    }
}    
