using SocialNetwork.Application.Common;
using SocialNetwork.Application.DTO;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Interfaces;

namespace SocialNetwork.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void PostMessage(string username, string content, DateTime timestamp)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user == null)
            {
                user = new User(username);
                _userRepository.AddUser(user);
            }

            user.PostMessage(content, timestamp);
        }

        public void FollowUser(string followerUsername, string followeeUsername)
        {
            var follower = _userRepository.GetUserByUsername(followerUsername);
            var followee = _userRepository.GetUserByUsername(followeeUsername);

            if (follower == null)
            {
                throw new UserNotFoundException(followerUsername);
            }
            else
            {
                bool alreadyFollowing = follower.Following.Contains(followee);

                if (alreadyFollowing)
                {
                    throw new UserAlreadyFollowedException(followerUsername, followeeUsername);
                }
            }

            if (followee == null)
            {
                throw new UserNotFoundException(followeeUsername);
            }

            follower.Follow(followee);
        }

        public IEnumerable<MessageDto> GetDashboard(string username)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user == null)
            {
                throw new UserNotFoundException(username);
            }

            var followingPosts = user.Following
                .SelectMany(f => f.Messages)
                .OrderBy(p => p.Timestamp)
                .Select(p => new MessageDto(p.Content, p.Timestamp, p.Username));

            return followingPosts;
        }
    }
}
