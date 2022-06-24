using System.ComponentModel.DataAnnotations;

namespace Armut.Messaging.Application.DTOs
{
    public class AuthRequest
    {
        [MinLength(3)]
        [MaxLength(100)]
        public string Username { get; set; }

        [MinLength(6)]
        [MaxLength(20)]
        public string Password { get; set; }
    }
}
