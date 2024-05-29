using SocialNetwork.Application.DTO;

namespace SocialNetwork.Application.Interfaces
{
    public interface IUserService
    {
        void PostMessage(string username, string content, DateTime timestamp);
        void FollowUser(string followerUsername, string followeeUsername);
        IEnumerable<MessageDto> GetDashboard(string username);
    }
}
