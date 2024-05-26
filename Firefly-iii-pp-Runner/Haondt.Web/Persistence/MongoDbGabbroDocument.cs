using MongoDB.Bson.Serialization.Attributes;

namespace Haondt.Web.Persistence
{
    [BsonIgnoreExtraElements]
    public class MongoDbDocument<T>
    {
        public required StorageKey Key { get; set; }
        public required T Value { get; set; }
    }
}
