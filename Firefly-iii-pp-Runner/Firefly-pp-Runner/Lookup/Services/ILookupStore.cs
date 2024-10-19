using Firefly_pp_Runner.Lookup.Reasons;
using Haondt.Core.Models;

namespace Firefly_pp_Runner.Lookup.Services
{
    public interface ILookupStore
    {
        Task UpsertPrimaryKeyValue(string key, string value);
        Task AddForeignKey(string primaryKey, string foreignKey);
        Task<List<string>> AutocompletePrimaryKeyAsync(string partialKey, int maxSuggestions);
        Task<List<string>> AutocompleteForeignKey(string partialKey, int maxSuggestions);
        Task DeletePrimaryKey(string key);
        Task DeleteForeignKey(string key);

        Task<List<string>> GetForeignKeys(string primaryKey);
        Task<Result<string, LookupResultReason>> GetValueAsync(string primaryKey);
        Task<Result<string, LookupResultReason>> GetValueFromForeignKey(string foreignKey);
    }
}
