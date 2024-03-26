using Microsoft.AspNetCore.Identity;

namespace Chat.Entities
{
    public class User : IdentityUser<long>
    {
        public List<Message>? Messages { get; set; }
    }
}
