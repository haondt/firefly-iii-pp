namespace Firefly_pp_Runner.Lookup.Services
{
    public interface ILookupStoreProvider
    {
        List<string> GetAvailableStores();
        ILookupStore GetStore(string storeName);
    }
}
