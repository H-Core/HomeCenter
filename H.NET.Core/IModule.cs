using System;

namespace H.NET.Core
{
    public interface IModule : IDisposable
    {
        string Name { get; }
        string Description { get; }

        ISettingsStorage Settings { get; }
        bool IsValid();

        event TextDelegate NewSpeech;
        event TextDelegate NewOutput;
        event TextDelegate NewCommand;
    }

    public delegate void TextDelegate(string text);
}
