using Armut.Messaging.Infrastructure.Persistence;

namespace Armut.Messaging.Domain.Models
{
    public class UserMessage : DocumentBase
    {
        public string? SourceUserName { get; set; }
        public string? TargetUserName { get; set; }
        public string? Message { get; set; }
    }
}