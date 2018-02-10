using VoiceActions.NET.Utilities;

namespace VoiceActions.NET.Storages
{
    public class InvariantDictionaryStorage<T> : InvariantStringDictionary<T>, IStorage<T>
    {
        public virtual void Load() { }
        public virtual void Save() { }
    }
}
