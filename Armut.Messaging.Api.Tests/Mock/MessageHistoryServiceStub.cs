using Armut.Messaging.Domain.Models;

namespace Armut.Messaging.Tests.Mock
{
    public static class MockDataProvider
    {
        public static List<UserMessage> MockMessages
        {
            get
            {
                return new List<UserMessage>
            {
                new UserMessage
                {
                    Id = "123",
                    SourceUserName = "ugur",
                    TargetUserName = "burak",
                    Message = "Hello burak"
                },
                new UserMessage
                {
                    Id = "124",
                    SourceUserName = "ugur",
                    TargetUserName = "melis",
                    Message = "Hello melis"
                },
                new UserMessage
                {
                    Id = "125",
                    SourceUserName = "melis",
                    TargetUserName = "ugur",
                    Message = "Hello ugur"
                },
                new UserMessage
                {
                    Id = "126",
                    SourceUserName = "melis",
                    TargetUserName = "burak",
                    Message = "Hello burak"
                },
                new UserMessage
                {
                    Id = "127",
                    SourceUserName = "burak",
                    TargetUserName = "ugur",
                    Message = "Hello ugur"
                },
                new UserMessage
                {
                    Id = "128",
                    SourceUserName = "burak",
                    TargetUserName = "melis",
                    Message = "Hello melis"
                },
            };
            }
        }
    }
}
