using Microsoft.AspNetCore.Identity;

namespace Chat.Services;

public interface ITokenService
{
    string CreateToken(IdentityUser<long> user, List<IdentityRole<long>> role);
}