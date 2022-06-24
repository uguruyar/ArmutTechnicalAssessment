using Armut.Messaging.Infrastructure.Persistence;
using Armut.Messaging.SharedKernel.Enums;

namespace Armut.Messaging.Domain.Models
{
    public class UserActivity : DocumentBase
    {
        public ActivityType ActivityType { get; set; }

        public string Description { get; set; }
    }
}
