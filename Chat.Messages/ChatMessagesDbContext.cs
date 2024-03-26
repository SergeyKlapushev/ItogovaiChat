using Chat.Messages.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Messages;

public class ChatMessagesDbContext : DbContext
{
    public ChatMessagesDbContext(DbContextOptions<ChatMessagesDbContext> options) : base(options)
    {
    }
    
    public DbSet<MessageEntity> Messages { get; set; }
}