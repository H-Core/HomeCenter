using System;
using VoiceActions.NET;

namespace HomeCenter.NET.Runners
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
