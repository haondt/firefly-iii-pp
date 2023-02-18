using Firefly_pp_Runner.Extensions;
using Firefly_pp_Runner.Settings;
using FireflyIIIpp.Core.Exceptions;
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

        public bool CollectionExists(string path)
        {
            return _storage.ContainsKey(path);
        }
        public void AssertCollectionExists(string path)
        {
            if (!CollectionExists(path))
                throw new NotFoundException(path);
        }

        public void AssertPathExists(string collection, string path)
        {
            AssertCollectionExists(collection);
            if (!_storage[collection].ContainsKey(path))
                throw new NotFoundException(path);
        }

        public Task CreateCollectionAsync(string collection)
        {
            _storage.Add(collection, new Dictionary<string, object>());
            return Task.CompletedTask;
        }

        public Task CreatePathAndWriteAsync<T>(string collection, string path, T obj)
        {
            _storage[collection].Add(path, obj);
            return Task.CompletedTask;
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
            AssertPathExists(collection, path);
            _storage[collection][path] = obj;
            return Task.CompletedTask;
        }
    }
}
