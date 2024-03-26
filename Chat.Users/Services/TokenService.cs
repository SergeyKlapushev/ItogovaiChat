using System.IdentityModel.Tokens.Jwt;
using Chat.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Chat.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ChatDbContext _context;

    public TokenService(IConfiguration configuration, ChatDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public string CreateToken(IdentityUser<long> user, List<IdentityRole<long>> roles)
    {
        var token = user
            .CreateClaims(roles)
            .CreateJwtToken(_configuration);
        var tokenHandler = new JwtSecurityTokenHandler();
        
        return tokenHandler.WriteToken(token);
    }
}