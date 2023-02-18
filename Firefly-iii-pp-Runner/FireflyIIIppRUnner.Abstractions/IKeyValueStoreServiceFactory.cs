using FireflyIIIppRunner.Abstractions.KeyValueStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIppRunner.Abstractions
{
    public interface IKeyValueStoreServiceFactory
    {
        public IKeyValueStoreService GetKeyValueStoreService(string store);
        public bool TryGetKeyValueStoreService(string store, out IKeyValueStoreService keyValueStoreService);
    }
}
