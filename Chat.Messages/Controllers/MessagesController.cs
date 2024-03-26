using System.Security.Claims;
using Chat.Messages.Entities;
using Chat.Messages.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.Messages.Controllers;

[ApiController]
[Route("messages")]
public class MessagesController : ControllerBase
{
    private readonly ChatMessagesDbContext _context;

    public MessagesController(ChatMessagesDbContext context)
    {
        _context = context;
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var recepientEmail = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Email)?.Value;
        var recepientUserId = long.Parse(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var messages = await _context.Messages
            .Where(x => (x.RecepientUserId == recepientUserId || x.RecepientEmail == recepientEmail) && !x.IsReceived)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsReceived = true;
        }

        await _context.SaveChangesAsync(); 
        
        return Ok(new {messages});
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Send(ShortMessageModel request)
    {
        var senderEmail = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Email)?.Value;
        var senderUserId = long.Parse(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
        
        var messageEntity = await _context.Messages.AddAsync(
            new MessageEntity
            {
                Text = request.Text,
                RecepientEmail = request.ReceipientEmail,
                RecepientUserId = request.RecepientUserId,
                SenderEmail = senderEmail,
                SenderUserId = senderUserId
            });

        await _context.SaveChangesAsync();

        return Ok(new {messageId = messageEntity.Entity.Id});
    }
}