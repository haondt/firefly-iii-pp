using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;

namespace Firefly_pp_Runner.Lookup.Services
{
    public interface ILookupStorage : IStorage
    {
        Task<Result<StorageResultReason>> AddForeignKey<T>(StorageKey<T> primaryKey, StorageKey<T> foreignKey);
        Task<List<StorageKey<T>>> AutocompleteForeignKey<T>(StorageKey<T> partialForeignKey, int maxResults);
        Task<List<StorageKey<T>>> AutocompletePrimaryKey<T>(StorageKey<T> partialPrimaryKey, int maxResults);
        Task<Result<int, StorageResultReason>> DeleteForeignKey<T>(StorageKey<T> foreignKey);
    }
}
