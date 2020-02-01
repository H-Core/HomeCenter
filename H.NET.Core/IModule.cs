﻿using System;
using H.NET.Core.Utilities;

namespace H.NET.Core
{
    public interface IModule : IDisposable
    {
        string Name { get; }
        string ShortName { get; }
        string UniqueName { get; set; }
        bool IsRegistered { get; set; }
        string Description { get; }

        ISettingsStorage Settings { get; }
        bool IsValid();

        event EventHandler<string> NewCommand;
        event EventHandler<TextDeferredEventArgs> NewCommandAsync;
        event EventHandler<IModule> SettingsSaved;

        void SaveSettings();

        string[] GetSupportedVariables();
        object GetModuleVariableValue(string name);
    }
}
