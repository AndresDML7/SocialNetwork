namespace SocialNetwork.Domain.Entities
{
    public class User
    {
        public string Username { get; set; }
        public List<Message> Messages { get; set; }
        public List<User> Following { get; set; }

        public User(string username)
        {
            Username = username;
            Messages = new List<Message>();
            Following = new List<User>();
        }

        public void PostMessage(string content, DateTime timestamp)
        {
            Messages.Add(new Message(content, timestamp, Username));
        }

        public void Follow(User user)
        {
            if (!Following.Contains(user))
            {
                Following.Add(user);
            }
        }
    }
}
