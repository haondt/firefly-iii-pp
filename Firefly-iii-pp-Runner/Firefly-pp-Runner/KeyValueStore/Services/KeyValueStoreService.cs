using Firefly_pp_Runner.Settings;
using FireflyIIIpp.Core.Exceptions;
using FireflyIIIppRunner.Abstractions;
using FireflyIIIppRunner.Abstractions.KeyValueStore;
using FireflyIIIppRunner.Abstractions.KeyValueStore.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.KeyValueStore.Services
{
    internal class KeyValueStoreStorage
    {
        public List<KeyValuePair<string, string>> Keys { get; set; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Values { get; set; } = new List<KeyValuePair<string, string>>();
    }

    internal class KeyValueStoreStore
    {
        public Dictionary<string, string> Keys { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }

    public class KeyValueStoreService : IKeyValueStoreService, IDisposable
    {
        private readonly KeyValueStoreSettings _settings;
        private KeyValueStoreStore _store;
        private readonly IPersistenceService _persistenceService;
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private Guid? _executionThread;

        public KeyValueStoreService(IOptions<KeyValueStoreSettings> options, IPersistenceService persistenceService)
        {
            _settings = options.Value;
            _persistenceService = persistenceService;
        }

        private async Task<KeyValueStoreStore> GetStore()
        {
            if (_store != null)
                return _store;

            if (_persistenceService.PathExists(_settings.Collection, _settings.Path))
            {
                await InternalReadFromStorage();
            }
            else
            {
                _store = new KeyValueStoreStore();
                await InternalWriteToStorage();
            }
            return _store;
        }

        private async Task InternalReadFromStorage()
        {
            _persistenceService.AssertPathExists(_settings.Collection, _settings.Path);
             var storage = await _persistenceService.ReadAsync<KeyValueStoreStorage>(_settings.Collection, _settings.Path);
            _store = new KeyValueStoreStore
            {
                Keys = storage.Keys.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Values = storage.Values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }

        private async Task InternalWriteToStorage()
        {
            await _persistenceService.WriteAsync(_settings.Collection, _settings.Path, new KeyValueStoreStorage
            {
                Keys = _store.Keys.OrderBy(i => i.Value)
                    .ToList(),
                Values = _store.Values.OrderBy(i => i.Key)
                    .ToList()
            });
        }

        private async Task TryAcquireSemaphoreAnd(Func<Task> func)
        {
            if (!await _semaphoreSlim.WaitAsync(1000))
                throw new Exception("Unable to acquire semaphore within the time limit");
            try
            {
                await func();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
        private async Task<T> TryAcquireSemaphoreAnd<T>(Func<Task<T>> func)
        {
            if (!await _semaphoreSlim.WaitAsync(1000))
                throw new Exception("Unable to acquire semaphore within the time limit");
            try
            {
                return await func();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public Task ReadFromStorage()
        {
            return TryAcquireSemaphoreAnd(InternalReadFromStorage);
        }

        public Task WriteToStorage()
        {
            return TryAcquireSemaphoreAnd(InternalWriteToStorage);
        }


        private async Task<List<string>> InternalGetKeys(string value)
        {
            var store = await GetStore();
            if (!store.Values.ContainsKey(value))
                throw new NotFoundException(value);
            var keys = store.Keys.Where(kvp => kvp.Value == value).Select(kvp => kvp.Key).ToList();
            if (keys.Count == 0)
                throw new Exception("KeyValueStore data is corrupted");
            return keys;
        }

        public Task<List<string>> GetKeys(string value)
        {
            return TryAcquireSemaphoreAnd(() => InternalGetKeys(value));
        }

        public  Task AddKey(string key, string value)
        {
            return TryAcquireSemaphoreAnd(async () =>
            {
                var store = await GetStore();
                if (store.Keys.ContainsKey(key))
                {
                    var valueKey = store.Keys[key];
                    if (store.Values.ContainsKey(valueKey))
                        throw new ConflictException($"Key \"{key}\" already mapped to value \"{valueKey}\"");
                    throw new Exception("KeyValueStore data is corrupted");
                }
                store.Keys[key] = value;
                if (!store.Values.ContainsKey(value))
                    store.Values[value] = _settings.DefaultValueValue;
                await InternalWriteToStorage();
            });
        }

        public Task DeleteKey(string key)
        {
            return TryAcquireSemaphoreAnd(async () =>
            {
                var store = await GetStore();
                if (!store.Keys.ContainsKey(key))
                    throw new NotFoundException(key);
                var valueKey = store.Keys[key];
                if (!store.Values.ContainsKey(valueKey))
                    throw new Exception("KeyValueStore data is corrupted");
                store.Keys.Remove(key);
                if (!store.Keys.Any(kvp => kvp.Value == valueKey))
                    store.Values.Remove(valueKey);
                await InternalWriteToStorage();
            });
        }

        public Task<string> GetValue(string value)
        {
            return TryAcquireSemaphoreAnd(async () =>
            {
                var store = await GetStore();
                if (!store.Values.ContainsKey(value))
                    throw new NotFoundException(value);
                return store.Values[value];
            });
        }

        public Task UpdateValue(string value, string valueValue)
        {
            return TryAcquireSemaphoreAnd(async () =>
            {
                var store = await GetStore();
                if (!store.Values.ContainsKey(value))
                    throw new NotFoundException(value);
                store.Values[value] = valueValue;
                await InternalWriteToStorage();
            });
        }

        public Task DeleteValue(string value)
        {
            return TryAcquireSemaphoreAnd(async () =>
            {
                var store = await GetStore();
                var keys = await InternalGetKeys(value);
                foreach (var key in keys)
                    store.Keys.Remove(key);
                store.Values.Remove(value);
                await InternalWriteToStorage();
            });
        }

        public Task<List<string>> AutocompleteValue(string partialValue)
        {
            return TryAcquireSemaphoreAnd(async () =>
            {
                var store = await GetStore();
                return store.Values.Keys
                    .Where(s => s.Contains(partialValue, StringComparison.InvariantCultureIgnoreCase))
                    .Take(_settings.AutocompleteMaxResults).ToList();
            });
        }

        public void Dispose()
        {
            _semaphoreSlim.Dispose();
        }
    }
}
