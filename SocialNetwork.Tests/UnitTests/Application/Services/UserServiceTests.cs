using Moq;
using SocialNetwork.Application.Common;
using SocialNetwork.Application.Services;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Interfaces;

namespace SocialNetwork.Tests.UnitTests.Application.Services
{
    public class UserServiceTests
    {
        [Fact]
        public void PostMessage_NewUser_AddsUserAndPostMessage()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetUserByUsername("Alfonso")).Returns((User)null);
            var userService = new UserService(userRepositoryMock.Object);
            var timestamp = DateTime.Now;

            // Act
            userService.PostMessage("Alfonso", "Mensaje de prueba", timestamp);

            // Assert
            userRepositoryMock.Verify(x => x.AddUser(It.IsAny<User>()), Times.Once);
            userRepositoryMock.Verify(x => x.AddUser(It.Is<User>(u => u.Username == "Alfonso")), Times.Once);
            userRepositoryMock.Verify(x => x.GetUserByUsername("Alfonso"), Times.Once);
        }

        [Fact]
        public void PostMessage_ExistingUser_PostsMessage()
        {
            // Arrange
            var existingUser = new User("Alicia");
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetUserByUsername("Alicia")).Returns(existingUser);
            var userService = new UserService(userRepositoryMock.Object);
            var timestamp = DateTime.Now;

            // Act
            userService.PostMessage("Alicia", "Mensaje de prueba", timestamp);

            // Assert
            Assert.Single(existingUser.Messages);
            Assert.Equal("Mensaje de prueba", existingUser.Messages.First().Content);
            Assert.Equal(timestamp, existingUser.Messages.First().Timestamp);
        }

        [Fact]
        public void FollowUser_ValidFollow_FollowsUser()
        {
            // Arrange
            var follower = new User("follower");
            var followee = new User("followee");
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetUserByUsername("follower")).Returns(follower);
            userRepositoryMock.Setup(x => x.GetUserByUsername("followee")).Returns(followee);
            var userService = new UserService(userRepositoryMock.Object);

            // Act
            userService.FollowUser("follower", "followee");

            // Assert
            Assert.Contains(followee, follower.Following);
        }

        [Fact]
        public void FollowUser_AlreadyFollowing_ThrowsUserAlreadyFollowedException()
        {
            // Arrange
            var follower = new User("follower");
            var followee = new User("followee");
            follower.Follow(followee);
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetUserByUsername("follower")).Returns(follower);
            userRepositoryMock.Setup(x => x.GetUserByUsername("followee")).Returns(followee);
            var userService = new UserService(userRepositoryMock.Object);

            // Act & Assert
            var ex = Assert.Throws<UserAlreadyFollowedException>(() => userService.FollowUser("follower", "followee"));
            Assert.Equal("follower ya está siguiendo a @followee", ex.Message);
        }

        [Fact]
        public void GetDashboard_UserNotFound_ThrowsUserNotFoundException()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetUserByUsername("Ivan")).Returns((User)null);
            var userService = new UserService(userRepositoryMock.Object);

            // Act & Assert
            var ex = Assert.Throws<UserNotFoundException>(() => userService.GetDashboard("Ivan"));
            Assert.Equal("No se encontró ningún usuario @Ivan", ex.Message);
        }

        [Fact]
        public void GetDashboard_UserFound_ReturnsDashboard()
        {
            // Arrange
            var followedUser1 = new User("Alfonso");
            followedUser1.PostMessage("Mensaje 1 de Alfonso", DateTime.Now);
            followedUser1.PostMessage("Mensaje 2 de Alfonso", DateTime.Now);

            var followedUser2 = new User("Ivan");
            followedUser2.PostMessage("Mensaje 1 de Ivan", DateTime.Now);

            var follower = new User("Alicia");
            follower.Follow(followedUser1);
            follower.Follow(followedUser2);

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetUserByUsername("Alicia")).Returns(follower);
            var userService = new UserService(userRepositoryMock.Object);

            // Act
            var dashboard = userService.GetDashboard("Alicia");

            // Assert
            Assert.Equal(3, dashboard.Count());

            // Verificar el contenido de los mensajes
            Assert.Contains("Mensaje 1 de Alfonso", dashboard.Select(m => m.Content));
            Assert.Contains("Mensaje 2 de Alfonso", dashboard.Select(m => m.Content));
            Assert.Contains("Mensaje 1 de Ivan", dashboard.Select(m => m.Content));
        }

    }
}
