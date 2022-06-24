using System.ComponentModel.DataAnnotations;

namespace Armut.Messaging.Application.DTOs
{
    public class MessageHistoryRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string TargetUserName { get; set; }
    }
}
