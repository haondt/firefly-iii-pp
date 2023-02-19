using FireflyIIIppRunner.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.Core.Tests.Utilities.Services
{
    public class FakePersistenceService : IPersistenceService
    {
        public Dictionary<string, Dictionary<string, object>> Store { get; set; } = new Dictionary<string, Dictionary<string, object>>();

        public void AssertCollectionExists(string path)
        {
            if(!Store.ContainsKey(path))
                throw new KeyNotFoundException();
        }

        public void AssertPathExists(string collection, string path)
        {
            AssertCollectionExists(collection);
            if(!Store[collection].ContainsKey(path))
                throw new KeyNotFoundException();
        }

        public bool CollectionExists(string path)
        {
            throw new NotImplementedException();
        }

        public Task CreateCollectionAsync(string collection)
        {
            throw new NotImplementedException();
        }

        public Task CreatePathAndWriteAsync<T>(string collection, string path, T obj)
        {
            throw new NotImplementedException();
        }

        public bool PathExists(string collection, string path)
        {
            return Store.TryGetValue(collection, out var result) && result.ContainsKey(path);
        }

        public Task<T> ReadAsync<T>(string collection, string path)
        {
            return Task.FromResult(Store.TryGetValue(collection, out var result) && result.TryGetValue(path, out var value) ? (T)value : throw new KeyNotFoundException());
        }

        public Task WriteAsync<T>(string collection, string path, T obj)
        {
            Store[collection][path] = obj;
            return Task.CompletedTask;
        }
    }
}
