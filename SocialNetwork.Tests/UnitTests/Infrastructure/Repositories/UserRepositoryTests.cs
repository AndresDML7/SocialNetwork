using Moq;
using SocialNetwork.Application.Services;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Interfaces;
using SocialNetwork.Infrastructure.Repositories;

namespace SocialNetwork.Tests.UnitTests.Infrastructure.Repositories
{
    public class UserRepositoryTests
    {
        [Fact]
        public void GetUserByUsername_UserExists_ReturnsUser()
        {
            // Arrange
            var userRepository = new UserRepository();
            var user = new User("Alfonso");
            userRepository.AddUser(user);

            // Act
            var result = userRepository.GetUserByUsername("Alfonso");

            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public void GetUserByUsername_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userRepository = new UserRepository();

            // Act
            var result = userRepository.GetUserByUsername("Ivan");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddUser_UserAddedSuccessfully()
        {
            // Arrange
            var userRepository = new UserRepository();
            var user = new User("Alicia");

            // Act
            userRepository.AddUser(user);

            // Assert
            var retrievedUser = userRepository.GetUserByUsername("Alicia");
            Assert.NotNull(retrievedUser);
            Assert.Equal(user, retrievedUser);
        }

        [Fact]
        public void UserFlow_PublishMessage_FollowUser_ViewDashboard()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var userService = new UserService(userRepositoryMock.Object);

            var publisher = new User("publisher");
            var follower = new User("follower");

            userRepositoryMock.Setup(x => x.GetUserByUsername("publisher")).Returns(publisher);
            userRepositoryMock.Setup(x => x.GetUserByUsername("follower")).Returns(follower);

            var messageContent = "Test message";
            var timestamp = DateTime.Now;

            // Act
            userService.PostMessage("publisher", messageContent, timestamp);

            userService.FollowUser("follower", "publisher");

            var dashboard = userService.GetDashboard("follower");

            // Assert
            Assert.Single(dashboard);
            Assert.Equal(messageContent, dashboard.First().Content);
            Assert.Equal(timestamp, dashboard.First().Timestamp);
        }

    }
}
