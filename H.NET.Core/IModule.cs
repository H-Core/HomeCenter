using System;
using H.NET.Core.Utilities;

namespace H.NET.Core
{
    public interface IModule : IDisposable
    {
        string Name { get; }
        string Description { get; }

        ISettingsStorage Settings { get; }
        bool IsValid();

        event TextDelegate NewCommand;
        event EventHandler<TextDeferredEventArgs> NewCommandAsync;
        event ModuleDelegate SettingsSaved;
    }

    public delegate void TextDelegate(string text);
    public delegate void ModuleDelegate(IModule module);
}
