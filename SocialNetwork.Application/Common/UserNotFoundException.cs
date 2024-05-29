namespace SocialNetwork.Application.Common
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string username)
            : base($"No se encontró ningún usuario @{username}")
        {
        }
    }
}
