using System;

namespace VoiceActions.NET.Runners
{
    public interface IRunner : IDisposable
    {
        void Run(string command);

        event EventHandler<VoiceActionsEventArgs> BeforeRun;
        event EventHandler<VoiceActionsEventArgs> AfterRun;

        string[] GetSupportedCommands();
        string GetSupportedCommandsText();
    }
}
