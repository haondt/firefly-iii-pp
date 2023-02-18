using Firefly_pp_Runner.Settings;
using FireflyIIIpp.Core.Exceptions;
using FireflyIIIppRunner.Abstractions;
using FireflyIIIppRunner.Abstractions.KeyValueStore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.KeyValueStore.Services
{
    public class KeyValueStoreServiceFactory : IKeyValueStoreServiceFactory
    {
        private readonly KeyValueStoreSettings _settings;
        private readonly IPersistenceService _persistenceService;
        private readonly Dictionary<string, KeyValueStoreService> _keyValueStores = new Dictionary<string,KeyValueStoreService>();
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public KeyValueStoreServiceFactory(IPersistenceService persistenceService,
            IOptions<KeyValueStoreSettings> options)
        {
            _settings = options.Value;
            _persistenceService = persistenceService;
        }

        public List<string> GetAvailableStores()
        {
            return _settings.Stores.Keys.ToList();
        }

        public async Task<IKeyValueStoreService> GetKeyValueStoreService(string store)
        {
            var (s, kvss) = await TryGetKeyValueStoreService(store);
            return s ? kvss : throw new NotFoundException(store);
        }


        public async Task<(bool Success, IKeyValueStoreService KeyValueStoreService)> TryGetKeyValueStoreService(string store)
        {
            if(!_keyValueStores.ContainsKey(store))
            {
                if (!_settings.Stores.ContainsKey(store))
                    return (false, null);
                _keyValueStores[store] = new KeyValueStoreService(
                    _settings.Collection,
                    _settings.AutocompleteMaxResults,
                    _settings.Stores[store],
                    _persistenceService);

                if (_settings.CreateCollections && !_persistenceService.CollectionExists(_settings.Collection))
                {
                    if (!await _semaphoreSlim.WaitAsync(1000))
                        throw new Exception("Unable to acquire semaphore within the time limit");
                    try { await _persistenceService.CreateCollectionAsync(_settings.Collection); }
                    finally { _semaphoreSlim.Release(); }
                }
            }
            return (true, _keyValueStores[store]);
        }
    }
}
