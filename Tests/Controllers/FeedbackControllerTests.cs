using Xunit; 
using Moq; 
using Microsoft.AspNetCore.Mvc; 
using System.Threading.Tasks;
using GrapheneTrace.Controllers;
using GrapheneTrace.Services.Interfaces;
using GrapheneTrace.Areas.Identity.Data;
using GrapheneTrace.Models.Feedback;
using Microsoft.AspNetCore.Identity;
using GrapheneTrace.Tests.Helpers; 
using System.Security.Claims;


namespace GrapheneTrace.Tests.Controllers
{


    public class FeedbackControllerTests
    {
        private readonly Mock<IFeedbackService> _mockFeedbackService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly FeedbackController _controller;
        
        public FeedbackControllerTests()
        {
            _mockFeedbackService = new Mock<IFeedbackService>();
            _mockUserManager = TestMockHelpers.MockUserManager<ApplicationUser>();
            _controller = new FeedbackController(_mockFeedbackService.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task DeleteFeedback_WithValidId_RedirectsToPatientHomeWithDataId()
        {
            // Arrange
            _mockFeedbackService.Setup(s => s.DeleteFeedback(1)).ReturnsAsync(123);

            // Act
            var result = await _controller.DeleteFeedback(1);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("PatientHome", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
            Assert.Equal(123, redirect.RouteValues["dataId"]);
        }

        [Fact]
        public async Task DeleteFeedback_WithNullDataId_RedirectsToPatientHomeWithoutDataId()
        {
            _mockFeedbackService.Setup(s => s.DeleteFeedback(2)).ReturnsAsync((int?)null);

            var result = await _controller.DeleteFeedback(2);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("PatientHome", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
            
            if (redirect.RouteValues != null)
            {
                Assert.False(redirect.RouteValues.ContainsKey("dataId"));
            }

        }

        [Fact]
        public async Task AddFeedback_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Error", "Invalid");
            var model = new NewFeedbackModel();

            var result = await _controller.AddFeedback(model);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddFeedback_ValidModel_RedirectsToPatientHomeWithDataId()
        {
            var user = new ApplicationUser { Id = 123, Name = "Test User", DateOfBirth = DateTime.Parse("2000-01-01") };
            
            _mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _mockFeedbackService.Setup(s => s.AddFeedback(It.IsAny<NewFeedbackModel>(), 123))
                .ReturnsAsync(999);

            var model = new NewFeedbackModel();

            var result = await _controller.AddFeedback(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("PatientHome", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
            Assert.Equal(999, redirect.RouteValues["dataId"]);
            
        }


        [Fact]
        public async Task AddFeedback_ValidModel_NullHeatmap_RedirectsToPatientHomeWithoutDataId()
        {
            var user = TestMockHelpers.RandomUser();

            _mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);
            
            _mockFeedbackService.Setup(s => s.AddFeedback(It.IsAny<NewFeedbackModel>(), 123))
                .ReturnsAsync((int?)null);

            var model = new NewFeedbackModel();

            var result = await _controller.AddFeedback(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("PatientHome", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);

            // More robust check
            if (redirect.RouteValues != null)
            {
                Assert.False(redirect.RouteValues.ContainsKey("dataId"), "Expected no dataId in route values.");
            }
        }




    }
}