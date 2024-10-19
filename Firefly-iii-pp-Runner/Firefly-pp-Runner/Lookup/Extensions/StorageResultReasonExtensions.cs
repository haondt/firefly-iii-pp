using Firefly_pp_Runner.Lookup.Reasons;
using Haondt.Persistence.Services;

namespace Firefly_pp_Runner.Lookup.Extensions
{
    public static class StorageResultReasonExtensions
    {
        public static LookupResultReason AsLookupReason(this StorageResultReason reason)
        {
            return reason switch
            {
                StorageResultReason.NotFound => LookupResultReason.NotFound,
                _ => throw new ArgumentException($"Unable to convert {nameof(StorageResultReason)} {reason}")
            };
        }
    }
}
