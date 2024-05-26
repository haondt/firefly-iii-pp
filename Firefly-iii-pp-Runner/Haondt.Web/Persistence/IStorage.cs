﻿namespace Haondt.Web.Persistence
{
    public interface IStorage
    {
        public Task<T> Get<T>(StorageKey<T> key);
        public Task<(bool Success, T? Value)> TryGet<T>(StorageKey<T> key);
        public Task<bool> ContainsKey(StorageKey key);
        public Task Set<T>(StorageKey<T> key, T value);
        public Task Delete(StorageKey key);
    }
}
