using Armut.Messaging.Infrastructure.Persistence;

namespace Armut.Messaging.Domain.Models
{
    public class Account : DocumentBase
    {
        public Account(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
