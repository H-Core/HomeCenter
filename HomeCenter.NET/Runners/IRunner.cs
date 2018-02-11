using System;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Storages;

namespace HomeCenter.NET.Runners
{
    public interface IRunner : IDisposable
    {
        IStorage<Command> Storage { get; set; }

        void Run(string key, string data);
        bool IsSupport(string key, string data);

        event EventHandler<RunnerEventArgs> NewSpeech;
        event EventHandler<RunnerEventArgs> NewOutput;
        event EventHandler<RunnerEventArgs> NewCommand;

        event EventHandler<RunnerEventArgs> BeforeRun;
        event EventHandler<RunnerEventArgs> AfterRun;

        string[] GetSupportedCommands();
        string GetSupportedCommandsText();
    }
}
