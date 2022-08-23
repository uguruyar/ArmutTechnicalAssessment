using Armut.Messaging.Infrastructure.Persistence;

namespace Armut.Messaging.Domain.Models
{
    public class User : DocumentBase
    {
        public User()
        {
            BlockedUserNames = new List<string>();
        }
        public string CurrentUser { get; set; }
        public List<string> BlockedUserNames { get; set; }
    }
}
