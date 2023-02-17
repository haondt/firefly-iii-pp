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

    public class KeyValueStoreService : IKeyValueStoreService
    {
        private readonly KeyValueStoreSettings _settings;
        private KeyValueStoreStore _store;
        private readonly IPersistenceService _persistenceService;

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
                await ReadFromStorage();
            }
            else
            {
                _store = new KeyValueStoreStore();
                await WriteToStorage();
            }
            return _store;
        }

        public async Task ReadFromStorage()
        {
            _persistenceService.AssertPathExists(_settings.Collection, _settings.Path);
             var storage = await _persistenceService.ReadAsync<KeyValueStoreStorage>(_settings.Collection, _settings.Path);
            _store = new KeyValueStoreStore
            {
                Keys = storage.Keys.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Values = storage.Values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }

        public async Task WriteToStorage()
        {
            await _persistenceService.WriteAsync(_settings.Collection, _settings.Path, new KeyValueStoreStorage
            {
                Keys = _store.Keys.OrderBy(i => i.Value)
                    .ToList(),
                Values = _store.Values.OrderBy(i => i.Key)
                    .ToList()
            });
        }


        public async Task<List<string>> GetKeys(string value)
        {
            var store = await GetStore();
            if (!store.Values.ContainsKey(value)) 
                throw new NotFoundException(value);
            var keys = store.Keys.Where(kvp => kvp.Value == value).Select(kvp => kvp.Key).ToList();
            if (keys.Count == 0)
                throw new Exception("KeyValueStore data is corrupted");
            return keys;
        }

        public async Task AddKey(string key, string value)
        {
            var store = await GetStore();
            if(store.Keys.ContainsKey(key))
            {
                var valueKey = store.Keys[key];
                if (store.Values.ContainsKey(valueKey))
                    throw new ConflictException($"Key \"{key}\" already mapped to value \"{store.Values[valueKey]}\"");
                throw new Exception("KeyValueStore data is corrupted");
            }
            store.Keys[key] = value;
            if (!store.Values.ContainsKey(value))
                store.Values[value] = _settings.DefaultValueValue;
            await WriteToStorage();
        }

        public async Task DeleteKey(string key)
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
            await WriteToStorage();
        }

        public async Task<string> GetValue(string value)
        {
            var store = await GetStore();
            if (!store.Values.ContainsKey(value)) 
                throw new NotFoundException(value);
            return store.Values[value];
        }

        public async Task UpsertValue(string value, string valueValue)
        {
            var store = await GetStore();
            store.Values[value] = valueValue;
            await WriteToStorage();
        }

        public async Task DeleteValue(string value)
        {
            var store = await GetStore();
            var keys = await GetKeys(value);
            foreach (var key in keys)
                store.Keys.Remove(key);
            store.Values.Remove(value);
            await WriteToStorage();
        }

        public async Task<List<string>> AutocompleteValue(string partialValue)
        {
            var store = await GetStore();
            return store.Values.Keys
                .Where(s => s.Contains(partialValue, StringComparison.InvariantCultureIgnoreCase))
                .Take(_settings.AutocompleteMaxResults).ToList();
        }
    }
}
