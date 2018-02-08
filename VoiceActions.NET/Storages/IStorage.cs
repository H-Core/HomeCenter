using System.Collections.Generic;

namespace VoiceActions.NET.Storages
{
    public interface IStorage<T> : IDictionary<string, T>
    {
    }
}
