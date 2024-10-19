using Firefly_pp_Runner.Lookup.Exceptions;
using Firefly_pp_Runner.Lookup.Extensions;
using Firefly_pp_Runner.Lookup.Reasons;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;

namespace Firefly_pp_Runner.Lookup.Services
{
    public class Store { }
    public class Lookup
    {
        public static StorageKey<Lookup> GetStorageKey(string store, string key) => StorageKey<Store>.Create(store).Extend<Lookup>(key);
        public static string GetKeySeed(StorageKey<Lookup> key) => key.Parts[^1].Value;
        public required string Value { get; set; }
    }

    public class LookupStore(
        string store,
        ILookupStorage storage) : ILookupStore
    {
        public async Task AddForeignKey(string primaryKey, string foreignKey)
        {
            var result = await storage.AddForeignKey(Lookup.GetStorageKey(store, primaryKey), Lookup.GetStorageKey(store, foreignKey));
            if (!result.IsSuccessful)
                throw new StorageException($"Failed to add foreign key {foreignKey} to primary key {primaryKey} due to reason {result.Reason}");
        }

        public async Task<List<string>> AutocompleteForeignKey(string partialKey, int maxSuggestions)
        {
            var keys = await storage.AutocompleteForeignKey(Lookup.GetStorageKey(store, partialKey), maxSuggestions);
            return keys.Select(Lookup.GetKeySeed).ToList();
        }

        public async Task<List<string>> AutocompletePrimaryKeyAsync(string partialKey, int maxSuggestions)
        {
            var keys = await storage.AutocompletePrimaryKey(Lookup.GetStorageKey(store, partialKey), maxSuggestions);
            return keys.Select(Lookup.GetKeySeed).ToList();
        }

        public Task DeleteForeignKey(string key)
        {
            throw new NotImplementedException();
        }

        public async Task DeletePrimaryKey(string key)
        {
            await storage.Delete(Lookup.GetStorageKey(store, key));
        }

        public Task<List<string>> GetForeignKeys(string primaryKey)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<string, LookupResultReason>> GetValueAsync(string primaryKey)
        {
            var result = await storage.Get(Lookup.GetStorageKey(store, primaryKey));
            if (!result.IsSuccessful)
                return new(result.Reason.AsLookupReason());
            return new(result.Value.Value);
        }

        public async Task<Result<string, LookupResultReason>> GetValueFromForeignKey(string foreignKey)
        {
            var results = await storage.GetMany(Lookup.GetStorageKey(store, foreignKey));
            if (results.Count == 0)
                return new(LookupResultReason.NotFound);
            return new(results[0].Value.Value);
        }

        public Task UpsertPrimaryKeyValue(string key, string value)
        {
            return storage.Set(Lookup.GetStorageKey(store, key), new Lookup { Value = value });
        }
    }
}
