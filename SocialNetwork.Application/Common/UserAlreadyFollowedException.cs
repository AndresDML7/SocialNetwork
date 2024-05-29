namespace SocialNetwork.Application.Common
{
    public  class UserAlreadyFollowedException : Exception
    {
        public UserAlreadyFollowedException(string follower, string followee) 
            : base($"{follower} ya está siguiendo a @{followee}") 
        {
        }
    }
}
