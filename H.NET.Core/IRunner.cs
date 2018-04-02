using System;
using H.NET.Core.Runners;

namespace H.NET.Core
{
    public interface IRunner : IModule
    {
        RunInformation Run(string key, string data);
        bool IsSupport(string key, string data);

        event EventHandler<RunnerEventArgs> BeforeRun;
        event EventHandler<RunnerEventArgs> AfterRun;

        string[] GetSupportedCommands();
        string[] GetVariables();
    }
}
