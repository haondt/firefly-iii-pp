using Firefly_iii_pp_Runner.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Firefly_iii_pp_Runner.API.Services
{
    public class MongoService
    {
        private readonly MongoDbSettings _settings;
        private readonly MongoClient _client;
        private readonly IMongoCollection<MongoDbObject> _testsCollection;

        public MongoService(IOptions<MongoDbSettings> options)
        {
            _settings = options.Value;
            var connectionString = $"mongodb://{_settings.Username}:{_settings.Password}@{_settings.Host}:{_settings.Port}";
            _client = new MongoClient(new MongoUrl(connectionString));
            var ppDb = _client.GetDatabase("pp");
            _testsCollection = ppDb.GetCollection<MongoDbObject>("tests");
        }

        public async Task<JToken?> GetTest(string id)
        {
            var cursor = await _testsCollection.FindAsync(x => x.Id == id);
            var body = (await cursor.SingleOrDefaultAsync())?.SerializedBody;
            if (body != null)
                return JsonConvert.DeserializeObject<JToken>(body);
            return null;
        }

        public Task SetTest(string id, JToken tests)
        {
            return _testsCollection.ReplaceOneAsync(
                o => o.Id == id,
                new MongoDbObject
                {
                    Id = id,
                    SerializedBody = JsonConvert.SerializeObject(tests)
                },
                new ReplaceOptions { IsUpsert = true });
        }
    }
}
