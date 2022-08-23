using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Armut.Messaging.Infrastructure.Persistence
{
    public abstract class DocumentBase
    {
        public DocumentBase()
        {
            CreatedAt = DateTime.UtcNow;
        }
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime CreatedAt { get; }
    }
}
