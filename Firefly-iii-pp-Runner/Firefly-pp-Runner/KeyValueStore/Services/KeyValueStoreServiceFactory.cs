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
        private readonly Dictionary<string, IKeyValueStoreService> _keyValueStores = new Dictionary<string,IKeyValueStoreService>();

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

        public IKeyValueStoreService GetKeyValueStoreService(string store)
        {
            if (TryGetKeyValueStoreService(store, out var storeService))
                return storeService;
            throw new NotFoundException(store);
        }


        public bool TryGetKeyValueStoreService(string store, out IKeyValueStoreService storeService)
        {
            if(!_keyValueStores.ContainsKey(store))
            {
                if (!_settings.Stores.ContainsKey(store))
                {
                    storeService = null;
                    return false;
                }
                _keyValueStores[store] = new KeyValueStoreService(
                    _settings.Collection,
                    _settings.AutocompleteMaxResults,
                    _settings.Stores[store],
                    _persistenceService);
            }
            storeService = _keyValueStores[store];
            return true;
        }
    }
}
