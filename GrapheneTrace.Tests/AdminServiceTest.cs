using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using GrapheneTrace.Services;
using GrapheneTrace.Areas.Identity.Data; 
using GrapheneTrace.Models.Admin;        
using GrapheneTrace.Data;
using GrapheneTrace.Enums;
using GrapheneTrace.Models.Database;
using Microsoft.EntityFrameworkCore;
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

        private static ApplicationDbContext GetNewDb(string dbname)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbname)
                .Options;
            return new ApplicationDbContext(options);
        }
        
        [Fact]
        public async Task GetLinkSelectionList_returns_empty_when_no_users_found_in_role()
        {
            var dbName = Guid.NewGuid().ToString();
            
            await using (var context = GetNewDb(dbName))
            {
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 99, PatientId = 10 });
                await context.SaveChangesAsync();
            }
            
            await using (var context = GetNewDb(dbName))
            {
                var userManagerMock = MockUserManager();
                var allPatients = new List<ApplicationUser> {};

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Patient)))
                    .ReturnsAsync(allPatients);
                
                var service = new AdminService(userManagerMock.Object, context);
                
                var result = await service.GetLinkSelectionList(UserType.Patient, 99, (LinkFilter)99);
                
                Assert.Equal(result, []);
            }
        }
        [Fact]
        public async Task GetLinkSelectionList_returns_empty_for_invalid_filter_type()
        {
            var dbName = Guid.NewGuid().ToString();
            
            await using (var context = GetNewDb(dbName))
            {
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 99, PatientId = 10 });
                await context.SaveChangesAsync();
            }
            
            await using (var context = GetNewDb(dbName))
            {
                var userManagerMock = MockUserManager();
                var allPatients = new List<ApplicationUser>
                {
                    new ApplicationUser { Id = 10, Name = "Linked Patient", Email = "linked@test.com", DateOfBirth = new DateTime(1990, 1, 1)},
                    new ApplicationUser { Id = 20, Name = "Available Patient", Email = "available@test.com", DateOfBirth = new DateTime(1990, 1, 2)},
                };

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Patient)))
                    .ReturnsAsync(allPatients);
                
                var service = new AdminService(userManagerMock.Object, context);
                
                var result = await service.GetLinkSelectionList(UserType.Patient, 99, (LinkFilter)99);
                
                Assert.Equal(result, []);
            }
        }

        [Fact]
        public async Task GetLinkSelectionList_returns_assigned_links_for_a_clinician()
        {
            var dbName = Guid.NewGuid().ToString();
            
            await using (var context = GetNewDb(dbName))
            {
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 99, PatientId = 10 });
                await context.SaveChangesAsync();
            }
            
            await using (var context = GetNewDb(dbName))
            {
                var userManagerMock = MockUserManager();
                var allPatients = new List<ApplicationUser>
                {
                    new ApplicationUser { Id = 10, Name = "Linked Patient", Email = "linked@test.com", DateOfBirth = new DateTime(1990, 1, 1)},
                    new ApplicationUser { Id = 20, Name = "Available Patient", Email = "available@test.com", DateOfBirth = new DateTime(1990, 1, 2)},
                };

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Patient)))
                    .ReturnsAsync(allPatients);
                
                var service = new AdminService(userManagerMock.Object, context);
                
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
            
            await using (var context = GetNewDb(dbName))
            {
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 10, PatientId = 99 });
                await context.SaveChangesAsync();
            }
            
            await using (var context = GetNewDb(dbName))
            {
                var userManagerMock = MockUserManager();
                var allPatients = new List<ApplicationUser>
                {
                    new ApplicationUser { Id = 10, Name = "Linked Clinician", Email = "linked@test.com", DateOfBirth = new DateTime(1990, 1, 1)},
                    new ApplicationUser { Id = 20, Name = "Available Clinician", Email = "available@test.com", DateOfBirth = new DateTime(1990, 1, 2)},
                };

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Clinician)))
                    .ReturnsAsync(allPatients);
                
                var service = new AdminService(userManagerMock.Object, context);
                
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
            
            await using (var context = GetNewDb(dbName))
            {
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 10, PatientId = 99 });
                await context.SaveChangesAsync();
            }
            
            await using (var context = GetNewDb(dbName))
            {
                var userManagerMock = MockUserManager();
                var allPatients = new List<ApplicationUser>
                {
                    new ApplicationUser { Id = 10, Name = "Linked Clinician", Email = "linked@test.com", DateOfBirth = new DateTime(1990, 1, 1)},
                    new ApplicationUser { Id = 20, Name = "Available Clinician", Email = "available@test.com", DateOfBirth = new DateTime(1990, 1, 2)},
                };

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Clinician)))
                    .ReturnsAsync(allPatients);
                
                var service = new AdminService(userManagerMock.Object, context);
                
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
            
            await using (var context = GetNewDb(dbName))
            {
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 99, PatientId = 10 });
                await context.SaveChangesAsync();
            }
            
            await using (var context = GetNewDb(dbName))
            {
                var userManagerMock = MockUserManager();
                var allPatients = new List<ApplicationUser>
                {
                    new ApplicationUser { Id = 10, Name = "Linked Patient", Email = "linked@test.com", DateOfBirth = new DateTime(1990, 1, 1)},
                    new ApplicationUser { Id = 20, Name = "Available Patient", Email = "available@test.com", DateOfBirth = new DateTime(1990, 1, 2)},
                };

                userManagerMock
                    .Setup(x => x.GetUsersInRoleAsync(nameof(UserType.Patient)))
                    .ReturnsAsync(allPatients);
                
                var service = new AdminService(userManagerMock.Object, context);
                
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
            await using (var context = GetNewDb(dbName))
            {
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 1, PatientId = 1 });
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 1, PatientId = 2 });
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 2, PatientId = 99 });
                await context.SaveChangesAsync();
            }

            await using (var context = GetNewDb(dbName))
            {
                var service = new AdminService(null!, context);
                var result = await service.GetAlreadyLinkedUsers(1, UserType.Patient);
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.Contains(1, result);
                Assert.Contains(2, result);
                Assert.DoesNotContain(99, result);

                
            }
        }

        [Fact]
        public async Task GetAlreadyLinkedUsers_should_return_linked_clinicians()
        {
            var dbName = Guid.NewGuid().ToString();
            await using (var context = GetNewDb(dbName)) 
            {
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 3, PatientId = 1 });
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 2, PatientId = 1 });
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 5, PatientId = 99 });
        
                await context.SaveChangesAsync();
            }

            await using (var context = GetNewDb(dbName))
            {
                var service = new AdminService(null!, context);
                var result = await service.GetAlreadyLinkedUsers(1, UserType.Clinician);
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.Contains(3, result);
                Assert.Contains(2, result);
                Assert.DoesNotContain(5, result);
            }
        }

        [Fact]
        public async Task UpdateLinks_Should_Work_For_Clinician_Managing_Patients()
        {
            var dbName = Guid.NewGuid().ToString();
            await using (var context = GetNewDb(dbName))
            {
                context.PatientClinician.Add(new PatientClinician { ClinicianId = 1, PatientId = 200 });
                await context.SaveChangesAsync();
            }
            
            await using (var context = GetNewDb(dbName))
            {
                var service = new AdminService(null!, context); // Replace 'YourService' with your actual class name
            
                var primaryUserId = 1; 
                var idsToAdd = new List<int> { 100 };
                var idsToRemove = new List<int> { 200 };
                const bool isManagingPatient = false; 

                await service.UpdatePatientClinicianLinks(idsToAdd, idsToRemove, primaryUserId, isManagingPatient);
            }
            
            await using (var context = GetNewDb(dbName))
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
            await using (var context = GetNewDb(dbName))
            {
                context.PatientClinician.Add(new PatientClinician { PatientId = 50, ClinicianId = 6 });
                await context.SaveChangesAsync();
            }
            
            await using (var context = GetNewDb(dbName))
            {
                var service = new AdminService(null!, context);
            
                var primaryUserId = 50; 
                var idsToAdd = new List<int> { 5 };
                var idsToRemove = new List<int> { 6 };
                var isManagingPatient = true; 

                await service.UpdatePatientClinicianLinks(idsToAdd, idsToRemove, primaryUserId, isManagingPatient);
            }
            
            await using (var context = GetNewDb(dbName))
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
            await using (var context = GetNewDb(dbName))
            {
                context.PatientClinician.Add(new PatientClinician { PatientId = 1, ClinicianId = 1 });
                await context.SaveChangesAsync();
            }

            await using (var context = GetNewDb(dbName))
            {
                var service = new AdminService(null!, context);
                await service.UpdatePatientClinicianLinks(new List<int>(), new List<int>(), 1, false);
            }
            
            await using (var context = GetNewDb(dbName))
            {
                Assert.Equal(1, await context.PatientClinician.CountAsync());
            }
        }
        
        [Fact]
        public async Task Update_User_Should_Return_False_When_User_Does_Not_Exist()
        {
            var userManager = MockUserManager();
            var adminService = new AdminService(userManager.Object, null!);
            var model = new EditUserViewModel { Id = 999 }; 
            
            userManager.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((ApplicationUser?)null);

            var result = await adminService.UpdateUser(model);

            Assert.False(result);
        }

        [Fact]
        public async Task Update_User_Should_Update_Fields_And_Roles()
        {
            var mockUserManager = MockUserManager();
            var service = new AdminService(mockUserManager.Object,null!);
            
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
            var service = new AdminService(mockUserManager.Object,null!);

            var model = new EditUserViewModel
            {
                Id = 1,
                Email = "test@test.com",
                NewPassword = "SuperSecretPassword123!", 
                ConfirmPassword = "SuperSecretPassword123!",
                SelectedRoles = new List<string>(),
                Name = "Test User",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            var existingUser = new ApplicationUser
            {
                Id = 1,
                Email = "test@test.com",
                Name = "Test User",
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
        [Fact]
        public async Task Update_User_Should_Not_Update_Password_if_it_does_not_match_password_confirm()
        {
            var mockUserManager = MockUserManager();
            var service = new AdminService(mockUserManager.Object, null!);

            var model = new EditUserViewModel
            {
                Id = 1,
                Email = "test@test.com",
                NewPassword = "SuperSecretPassword123!", 
                ConfirmPassword = "SuperSecretPassword123",
                SelectedRoles = new List<string>(),
                Name = "Test User",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            var existingUser = new ApplicationUser
            {
                Id = 1,
                Email = "test@test.com",
                Name = "Test User",
                DateOfBirth = new DateTime(1990, 1, 1)
            };
            
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
            var mockUserManager = MockUserManager();
            var service = new AdminService(mockUserManager.Object, null!);

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
            
            mockUserManager.Setup(x => x.AddPasswordAsync(existingUser, "BadPassword"))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Too weak" }));
            
            var result = await service.UpdateUser(model);
            Assert.False(result); 
            mockUserManager.Verify(x => x.UpdateAsync(existingUser), Times.Never);
        }
    }
}    
