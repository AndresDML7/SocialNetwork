namespace SocialNetwork.Application.DTO
{
    public class MessageDto
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }

        public MessageDto(string content, DateTime timestamp, string username)
        {
            Content = content;
            Timestamp = timestamp;
            Username = username;
        }

        public MessageDto()
        {
        }
    }

}
