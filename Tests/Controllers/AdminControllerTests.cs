using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GrapheneTrace.Areas.Identity.Data; 
using GrapheneTrace.Tests.Helpers;
using GrapheneTrace.Controllers;
using GrapheneTrace.Models;
using Xunit;

namespace GrapheneTrace.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockUserManager = TestMockHelpers.MockUserManager<ApplicationUser>();
            _mockSignInManager = TestMockHelpers.MockSignInManager(_mockUserManager);
            _controller = new AccountController(_mockUserManager.Object, _mockSignInManager.Object);
        }

        [Fact]
        public async Task Can_Create_New_User(){}

        public async Task Can_Edit_User()
        {
        }

        public async Task Can_Delete_User()
        {
        }
        
        
    }
}
