using Moq;
using SocialNetwork.Application.Services;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Interfaces;

namespace SocialNetwork.Tests.IntegrationTests
{
    public class UserFlowIntegrationTests
    {
        [Fact]
        public void UserFlow_PublishMessage_FollowUser_ViewDashboard()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var userService = new UserService(userRepositoryMock.Object);

            var publisher = new User("Alfonso");
            var follower = new User("Alicia");

            userRepositoryMock.Setup(x => x.GetUserByUsername("Alfonso")).Returns(publisher);
            userRepositoryMock.Setup(x => x.GetUserByUsername("Alicia")).Returns(follower);

            var messageContent = "Mensaje de prueba";
            var timestamp = DateTime.Now;

            // Act
            userService.PostMessage("Alfonso", messageContent, timestamp);

            userService.FollowUser("Alicia", "Alfonso");

            var dashboard = userService.GetDashboard("Alicia");

            // Assert
            Assert.Single(dashboard);
            Assert.Equal(messageContent, dashboard.First().Content);
            Assert.Equal(timestamp, dashboard.First().Timestamp);
        }
    }
}
