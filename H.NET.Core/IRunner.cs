using System;
using H.NET.Core.Runners;

namespace H.NET.Core
{
    public interface IRunner : IModule
    {
        void Run(string key, Command command);
        bool IsSupport(string key, Command command);

        event EventHandler<RunnerEventArgs> NewSpeech;
        event EventHandler<RunnerEventArgs> NewOutput;
        event EventHandler<RunnerEventArgs> NewCommand;

        event EventHandler<RunnerEventArgs> BeforeRun;
        event EventHandler<RunnerEventArgs> AfterRun;

        string[] GetSupportedCommands();
    }
}
