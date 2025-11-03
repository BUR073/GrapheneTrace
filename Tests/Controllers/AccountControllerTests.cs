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
    public class AccountControllerTests
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
        public async Task Can_Login_With_Valid_Credentials()
        {
            // Arrange
            var model = new LoginViewModel { Email = "test@example.com", Password = "Password123", RememberMe = false };

            _mockSignInManager
                .Setup(x => x.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success); // Fully qualified

            // Act
            var result = await _controller.Login(model);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task Cant_Login_With_Invalid_Credentials()
        {
            // Arrange
            var model = new LoginViewModel { Email = "wrong@example.com", Password = "WrongPass", RememberMe = false };

            _mockSignInManager
                .Setup(x => x.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Fully qualified

            // Act
            var result = await _controller.Login(model);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var returnedModel = Assert.IsType<LoginViewModel>(view.Model);

            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
        }
    }
}
