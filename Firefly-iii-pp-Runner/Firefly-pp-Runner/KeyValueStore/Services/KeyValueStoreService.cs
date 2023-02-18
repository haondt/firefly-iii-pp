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
        private KeyValueStoreStore _store;
        private readonly string _collection;
        private readonly int _autoCompleteMaxResults;
        private readonly KeyValueStoreStoreSettings _settings;
        private readonly IPersistenceService _persistenceService;
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public KeyValueStoreService(
            string collection,
            int autoCompleteMaxResults,
            KeyValueStoreStoreSettings settings,
            IPersistenceService persistenceService)
        {
            _collection = collection;
            _autoCompleteMaxResults = autoCompleteMaxResults;
            _settings = settings;
            _persistenceService = persistenceService;
        }

        private async Task<KeyValueStoreStore> GetStore()
        {
            if (_store != null)
                return _store;

            if (_persistenceService.PathExists(_collection, _settings.Path))
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
        private async Task<KeyValueStoreStore> SafeGetStore()
        {
            if (_store != null)
                return _store;

            return await TryAcquireSemaphoreAnd(async () =>
            {
                if (_store != null)
                    return _store;

                if (_persistenceService.PathExists(_collection, _settings.Path))
                {
                    await InternalReadFromStorage();
                }
                else
                {
                    _store = new KeyValueStoreStore();
                    await InternalWriteToStorage();
                }

                return _store;
            });
        }

        private async Task InternalReadFromStorage()
        {
            if (_settings.CreatePaths && _persistenceService.CollectionExists(_collection) && !_persistenceService.PathExists(_collection, _settings.Path))
                await _persistenceService.CreatePathAndWriteAsync(_collection, _settings.Path, new KeyValueStoreStorage());

            _persistenceService.AssertPathExists(_collection, _settings.Path);
             var storage = await _persistenceService.ReadAsync<KeyValueStoreStorage>(_collection, _settings.Path);
            _store = new KeyValueStoreStore
            {
                Keys = storage.Keys.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Values = storage.Values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }

        private async Task InternalWriteToStorage()
        {
            Func<string, string, KeyValueStoreStorage, Task> writeFunc = _persistenceService.WriteAsync;
            if (_settings.CreatePaths && _persistenceService.CollectionExists(_collection) && !_persistenceService.PathExists(_collection, _settings.Path))
                writeFunc = _persistenceService.CreatePathAndWriteAsync;

            await writeFunc(_collection, _settings.Path, new KeyValueStoreStorage
            {
                Keys = _store.Keys.OrderBy(i => i.Value).ToList(),
                Values = _store.Values.OrderBy(i => i.Key).ToList()
            });
        }

        private async Task TryAcquireSemaphoreAnd(Func<Task> func)
        {
            if (!await _semaphoreSlim.WaitAsync(1000))
                throw new Exception("Unable to acquire semaphore within the time limit");
            try { await func(); }
            finally { _semaphoreSlim.Release(); }
        }
        private async Task<T> TryAcquireSemaphoreAnd<T>(Func<Task<T>> func)
        {
            if (!await _semaphoreSlim.WaitAsync(1000))
                throw new Exception("Unable to acquire semaphore within the time limit");
            try { return await func(); }
            finally { _semaphoreSlim.Release(); }
        }

        public Task ReadFromStorage()
        {
            return TryAcquireSemaphoreAnd(InternalReadFromStorage);
        }

        public Task WriteToStorage()
        {
            return TryAcquireSemaphoreAnd(InternalWriteToStorage);
        }


        private async Task<(bool Success, List<string> Keys)> InternalGetKeys(string value, KeyValueStoreStore store)
        {
            if (!store.Values.ContainsKey(value))
                return (false, null);
            var keys = store.Keys.Where(kvp => kvp.Value == value).Select(kvp => kvp.Key).ToList();
            if (keys.Count == 0)
                throw new Exception("KeyValueStore data is corrupted");
            return (true, keys);
        }

        public async Task<(bool Success, List<string> Keys)> GetKeys(string value)
        {
            var store = await SafeGetStore();
            return await InternalGetKeys(value, store);
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
                var (success, keys) = await InternalGetKeys(value, store);
                if (!success)
                    throw new NotFoundException(value);
                foreach (var key in keys)
                    store.Keys.Remove(key);
                store.Values.Remove(value);
                await InternalWriteToStorage();
            });
        }

        public async Task<List<string>> AutocompleteValue(string partialValue)
        {
            if (string.IsNullOrWhiteSpace(partialValue))
                return new List<string>();
            var store = await SafeGetStore();
            return store.Values.Keys
                .Where(s => s.Contains(partialValue, StringComparison.InvariantCultureIgnoreCase))
                .Take(_autoCompleteMaxResults).ToList();
        }

        public void Dispose()
        {
            _semaphoreSlim.Dispose();
        }

        public Task<string> GetDefaultValueValue()
        {
            return Task.FromResult(_settings.DefaultValueValue);
        }

        public async Task<(bool Success, string Reason, string Value)> GetKeyValue(string key)
        {
            var store = await SafeGetStore();
            if (store.Keys.TryGetValue(key, out var value))
                return (true, null, value);
            return (false, $"Key not found", null);
        }

        public async Task<(bool Success, string ValueValue)> GetValueValue(string value)
        {
            var store = await SafeGetStore();
            if (!store.Values.ContainsKey(value))
                return (false, null);
            return (true, store.Values[value]);
        }

        public async Task<(bool Success, string Reason, string Value, string ValueValue)> GetKeyValueValue(string key)
        {
            var store = await SafeGetStore();
            if (store.Keys.TryGetValue(key, out var value))
                if (store.Values.TryGetValue(value, out var valueValue))
                    return (true, null, value, valueValue);
                else
                    throw new Exception("KeyValueStore data is corrupted");
            return (false, $"Key not found", null, null);
        }
    }
}
