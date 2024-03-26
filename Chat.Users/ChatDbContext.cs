using Chat.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chat
{
    public class ChatDbContext(DbContextOptions<ChatDbContext> options)
        : IdentityDbContext<IdentityUser<long>, IdentityRole<long>, long>(options)
    {
        public DbSet<Message> Messages { get; set; } = null!;
    }
}
