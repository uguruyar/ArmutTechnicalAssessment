using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Armut.Messaging.Infrastructure.Persistence
{
    public abstract class DocumentBase
    {
        public DateTime CreatedAt { get; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DocumentBase()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
