using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialNetwork.Api.Controllers;
using SocialNetwork.Application.Common;
using SocialNetwork.Application.DTO;
using SocialNetwork.Application.Interfaces;

namespace SocialNetwork.Tests.UnitTests.Api.Controllers
{
    public class UserControllerTests
    {
        [Fact]
        public void PostMessage_ValidRequest_ReturnsOk()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = new UserController(userServiceMock.Object);
            var request = new MessageDto("Mensaje de prueba", DateTime.Now, "Alfonso");

            // Act
            var result = controller.PostMessage(request);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void FollowUser_ValidRequest_ReturnsOk()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.FollowUser("follower", "followee")).Verifiable();
            var controller = new UserController(userServiceMock.Object);

            // Act
            var result = controller.FollowUser("follower", "followee");

            // Assert
            Assert.IsType<OkResult>(result);
            userServiceMock.Verify();
        }

        [Fact]
        public void FollowUser_UserNotFoundException_ReturnsNotFound()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.FollowUser("follower", "followee")).Throws(new UserNotFoundException("followee"));
            var controller = new UserController(userServiceMock.Object);

            // Act
            var result = controller.FollowUser("follower", "followee");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No se encontró ningún usuario @followee", notFoundResult.Value);
        }

        [Fact]
        public void FollowUser_UserAlreadyFollowedException_ReturnsBadRequest()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.FollowUser("follower", "followee")).Throws(new UserAlreadyFollowedException("follower", "followee"));
            var controller = new UserController(userServiceMock.Object);

            // Act
            var result = controller.FollowUser("follower", "followee");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("follower ya está siguiendo a @followee", badRequestResult.Value);
        }

        [Fact]
        public void GetDashboard_ValidRequest_ReturnsOk()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetDashboard("Alfonso")).Returns(new MessageDto[] { new MessageDto() });
            var controller = new UserController(userServiceMock.Object);

            // Act
            var result = controller.GetDashboard("Alfonso");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<MessageDto>>(okResult.Value);
        }

        [Fact]
        public void GetDashboard_UserNotFoundException_ReturnsNotFound()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetDashboard("Alfonso")).Throws(new UserNotFoundException("Alfonso"));
            var controller = new UserController(userServiceMock.Object);

            // Act
            var result = controller.GetDashboard("Alfonso");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No se encontró ningún usuario @Alfonso", notFoundResult.Value);
        }
    }
}
