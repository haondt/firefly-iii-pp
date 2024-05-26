using Haondt.Web.Persistence;

namespace Haondt.Web.Liftetime
{
    public class LifetimeHookService(IEnumerable<ILifetimeHook> hooks)
    {
        public Task FireAsync<T>(Func<T, Task> func) where T : ILifetimeHook =>
            Task.WhenAll(hooks
                .Where(h => h is T)
                .Cast<T>()
                .Select(func));
    }
}
