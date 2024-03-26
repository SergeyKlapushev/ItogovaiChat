namespace Chat.Messages.Entities;

public class MessageEntity : BaseEntity
{
    public string? Text { get; set; }
    public long? SenderUserId { get; set; }
    public string SenderEmail { get; set; }
    public long? RecepientUserId { get; set; }
    public string RecepientEmail { get; set; }
    public bool IsReceived { get; set; }
}