using Microsoft.Extensions.Options;

namespace Firefly_pp_Runner.Lookup.Services
{
    public class LookupStoreProvider(
        IOptions<LookupSettings> options,
        ILookupStorage storage) : ILookupStoreProvider
    {
        public ILookupStore GetStore(string storeName)
        {
            if (!options.Value.Stores.Contains(storeName))
                throw new InvalidOperationException($"No such store: {storeName}");

            return new LookupStore(storeName, storage);
        }

        public List<string> GetAvailableStores()
        {
            return options.Value.Stores;
        }
    }
}
