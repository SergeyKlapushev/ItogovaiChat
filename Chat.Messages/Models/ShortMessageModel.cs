namespace Chat.Messages.Models;

public class ShortMessageModel
{
    public string? Text { get; set; }
    public string? ReceipientEmail { get; set; }
    public long? RecepientUserId { get; set; }
}