using System;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public interface IRunner : IDisposable
    {
        void Run(string key, Command command);
        bool IsSupport(string key, Command command);

        event EventHandler<RunnerEventArgs> NewSpeech;
        event EventHandler<RunnerEventArgs> NewOutput;
        event EventHandler<RunnerEventArgs> NewCommand;

        event EventHandler<RunnerEventArgs> BeforeRun;
        event EventHandler<RunnerEventArgs> AfterRun;

        string[] GetSupportedCommands();
        string GetSupportedCommandsText();
    }
}
