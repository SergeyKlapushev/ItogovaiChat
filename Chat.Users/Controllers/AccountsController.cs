using System.Security.Claims;
using Chat.Models.Identity;
using Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly UserManager<IdentityUser<long>> _userManager;
    private readonly ChatDbContext _context;
    private readonly ITokenService _tokenService;

    public AccountsController(ITokenService tokenService, ChatDbContext context, UserManager<IdentityUser<long>> userManager)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var managedUser = await _userManager.FindByEmailAsync(request.Email);
        
        if (managedUser == null)
        {
            return BadRequest("Bad credentials");
        }
        
        var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
        
        if (!isPasswordValid)
        {
            return BadRequest("Bad credentials");
        }
        
        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        
        if (user is null)
            return Unauthorized();

        var roleIds = await _context.UserRoles.Where(r => r.UserId == user.Id)
            .Select(x => x.RoleId).ToListAsync();
        var roles = _context.Roles.Where(x => roleIds.Contains(x.Id)).ToList();
        
        var accessToken = _tokenService.CreateToken(user, roles);
        
        return Ok(new AuthResponse
        {
            Username = user.UserName!,
            Email = user.Email!,
            Token = accessToken,
        });
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterMember([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(request);
        
        var user = new IdentityUser<long> { Email = request.Email, UserName = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        if (result.Succeeded)
        {
            var findUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (findUser == null) throw new Exception($"User {request.Email} not found");
            
            await _userManager.AddToRoleAsync(findUser, RoleConsts.Member);
            
            return await Authenticate(new AuthRequest
            {
                Email = request.Email,
                Password = request.Password
            });
        }

        return BadRequest(request);
    }

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(request);
        
        var user = new IdentityUser<long> { Email = request.Email, UserName = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        if (result.Succeeded)
        {
            var findUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (findUser == null) throw new Exception($"User {request.Email} not found");
            
            await _userManager.AddToRoleAsync(findUser, RoleConsts.Administrator);
            
            return await Authenticate(new AuthRequest
            {
                Email = request.Email,
                Password = request.Password
            });
        }

        return BadRequest(request);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        
        return Ok(new {users});
    }

    [Authorize]
    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteByEmail(string email)
    {
        var role = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

        if (role?.Value.Contains(RoleConsts.Administrator) != true)
        {
            return Forbid();
        }

        var emailClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        
        if (emailClaim?.Value == email)
        {
            return BadRequest("Вы не можете удалить себя");
        }
        
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
        {
            return BadRequest($"Пользователь с email {email} не найден!");
        }
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return Ok();
    }

    [Authorize]
    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        
        return Ok(new {id = claim?.Value});
    }
}