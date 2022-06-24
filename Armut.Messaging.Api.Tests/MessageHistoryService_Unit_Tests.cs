using Armut.Messaging.Application.DTOs;
using Armut.Messaging.Application.Services.Abstract;
using Armut.Messaging.Application.Services.Concrete;
using Armut.Messaging.Domain.Models;
using Armut.Messaging.Tests.Mock;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;

namespace Armut.Messaging.Tests
{
    public class MessageHistoryService_Unit_Tests
    {
        private readonly IMessageHistoryService _service;

        private readonly Mock<IMongoDatabase> _mockDatabase;

        private readonly Mock<IMongoCollection<UserMessage>> _mockCollection;

        public MessageHistoryService_Unit_Tests()
        {
            _mockDatabase = new Mock<IMongoDatabase>();

            _mockCollection = new Mock<IMongoCollection<UserMessage>>();
            _mockDatabase.SetupGet(x => x.DatabaseNamespace).Returns(new DatabaseNamespace("test"));
            _mockDatabase.Setup(x => x.GetCollection<UserMessage>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>())).Returns(_mockCollection.Object);


            _service = new MessageHistoryService(_mockDatabase.Object);
        }


        [Theory]
        [InlineData("burak","ugur", 1)]
        [InlineData("ugur","burak", 1)]
        public async Task GetMessageHistoryAsync_Should_Return_MessageHistory(string sourceUserName, string targetUserName, int messageCount)
        {

            // Arrange
            var cursor = MockCursor(MockDataProvider.MockMessages);

            _mockCollection.Setup(x =>
                x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(), It.IsAny<FindOptions<UserMessage>>(), It.IsAny<CancellationToken>()))
                    .Returns((FilterDefinition<UserMessage> definition, FindOptions<UserMessage> options, CancellationToken cancellationToken) =>
                    {
                        if (FilterDefinition<UserMessage>.Empty == definition || definition == null)
                        {
                            return Task.FromResult(cursor);
                        }

                        Expression<Func<UserMessage, bool>> expression = ((dynamic)definition).Expression;

                        var result = MockDataProvider.MockMessages.Where(expression.Compile()).ToList();

                        return Task.FromResult(MockCursor(result));
                    });

            // Act
            var result = await _service.GetMessageHistoryAsync(sourceUserName, targetUserName, default);

            // Assert
            result.Count.Should().Be(messageCount);
        }

        private IAsyncCursor<TDocument> MockCursor<TDocument>(IEnumerable<TDocument> documents)
            where TDocument : class
        {
            var mockCursor = new Mock<IAsyncCursor<TDocument>>() { CallBase = true };
            mockCursor.Setup(_ => _.Current).Returns(documents);

            mockCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            return mockCursor.Object;
        }
    }
}