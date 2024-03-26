namespace Chat.Messages.Models;

public class MessageModel : ShortMessageModel
{
    public string? SenderEmail { get; set; } = null!;
    public long Id { get; set; }
}