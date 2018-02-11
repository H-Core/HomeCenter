namespace VoiceActions.NET.Storages
{
    public static class StorageExtensions
    {
        public static T GetOrAdd<T>(this IStorage<T> storage, string key, T value = default(T))
        {
            if (!storage.ContainsKey(key))
            {
                storage[key] = value;
            }

            return storage[key];
        }
    }
}
