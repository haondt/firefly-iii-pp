using Firefly_pp_Runner.Extensions;
using Firefly_pp_Runner.Settings;
using FireflyIIIppRunner.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Persistence
{
    public class MemoryPersistenceService : IPersistenceService
    {
        public Dictionary<string, Dictionary<string, object>> _storage = new Dictionary<string, Dictionary<string,object>>();

        public MemoryPersistenceService(IOptions<MemoryPersistenceServiceSettings> options)
        {
            foreach (var kvp in options.Value.ExistingCollections)
            {
                if (!_storage.TryGetValue(kvp.Key, out var collection))
                    collection = new Dictionary<string, object>();
                foreach(var path in kvp.Value)
                    collection[path] = new Dictionary<string, object>();
            }
        }

        public void AssertCollectionExists(string path)
        {
            if (!_storage.ContainsKey(path))
                throw new KeyNotFoundException(path);
        }

        public void AssertPathExists(string collection, string path)
        {
            AssertCollectionExists(collection);
            if (!_storage[collection].ContainsKey(path))
                throw new KeyNotFoundException(path);
        }

        public bool PathExists(string collection, string path)
        {
            return _storage.TryGetValue(collection, out var result) && result.ContainsKey(path);
        }

        public Task<T> ReadAsync<T>(string collection, string path)
        {
            return Task.FromResult((T)_storage[collection][path]);
        }

        public Task WriteAsync<T>(string collection, string path, T obj)
        {
            _storage[collection][path] = obj;
            return Task.CompletedTask;
        }
    }
}
