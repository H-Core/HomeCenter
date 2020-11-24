using H.NET.Core.Utilities;

namespace H.NET.Core.Storages
{
    public class InvariantDictionaryStorage<T> : InvariantStringDictionary<T>, IStorage<T>
    {
        public virtual void Load() { }
        public virtual void Save() { }
    }
}
