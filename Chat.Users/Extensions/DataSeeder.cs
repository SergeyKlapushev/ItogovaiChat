using Chat.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Chat.Extensions;

public class DataSeeder
{
    private readonly ChatDbContext _context;

    public DataSeeder(ChatDbContext context)
    {
        _context = context;
    }

    public async Task Seed()
    {
        if(_context.Roles.Any()) return;
        
        var roles = new [] { new IdentityRole<long>
        {
            Id = 1,
            Name = RoleConsts.Member,
            NormalizedName = RoleConsts.Member.ToUpperInvariant()
        }, new IdentityRole<long>
        {
            Id = 3,
            Name = RoleConsts.Administrator,
            NormalizedName = RoleConsts.Administrator.ToUpperInvariant()
        }};

        if (!_context.Roles.Any())
        {
            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
        }
    }
}