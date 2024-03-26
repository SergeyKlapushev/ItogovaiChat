namespace Chat.Entities
{
    public class Message : BaseEntity
    {
        public string? Text { get; set; }
        public bool IsRead { get; set; } 
        public long? UserId { get; set; }
        public User? User { get; set; }
    }
}
