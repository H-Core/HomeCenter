using System;

namespace H.NET.Core
{
    public interface IModule : IDisposable
    {
        string Name { get; }
        string Description { get; }

        ISettingsStorage Settings { get; }
        bool IsValid();
    }
}
