using System.Text.Json.Serialization;

namespace SocialNetwork.Domain.Entities
{
    public class Message
    {
        public string Content { get; }
        public DateTime Timestamp { get; }
        public string Username { get; set; }

        public Message(string content, DateTime timestamp, string username)
        {
            Content = content;
            Timestamp = timestamp;
            Username = username;
        }
    }
}
