/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using H.Core;
using H.Core.Converters;
using H.Core.Extensions;
using H.Core.Managers;
using H.Core.Recorders;
using H.Core.Runners;
using H.Core.Searchers;
using H.Core.Synthesizers;
using H.NET.Utilities.Plugins;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Services
{
    public class ModuleService : IDisposable
    {
        #region Properties

        private Settings Settings { get; }
        private BaseManager Manager { get; }

        private PluginsManager<IModule> PluginsManager { get; }

        public IRecorder? Recorder => PluginsManager.GetPlugin<IRecorder>(Settings.Recorder)?.Value;
        public ISearcher? Searcher => PluginsManager.GetPlugin<ISearcher>(Settings.Searcher)?.Value;
        public IConverter? Converter => PluginsManager.GetPlugin<IConverter>(Settings.Converter)?.Value;
        public ISynthesizer? Synthesizer => PluginsManager.GetPlugin<ISynthesizer>(Settings.Synthesizer)?.Value;

        public List<IConverter> AlternativeConverters => Settings.UseAlternativeConverters
            ? PluginsManager.GetEnabledPlugins<IConverter>()
                  .Where(pair => !string.Equals(pair.Key, Settings.Converter))
                  .Select(pair => pair.Value.Value)
                  .ToList() 
            : new List<IConverter>();

        public List<IRunner> Runners => PluginsManager.GetEnabledPlugins<IRunner>().Select(i => i.Value.Value).ToList();

        public List<IModule> Modules => PluginsManager.GetEnabledPlugins<IModule>().Select(i => i.Value.Value).ToList();

        public List<Type> AvailableTypes => PluginsManager.AvailableTypes;
        public Dictionary<string, Assembly> ActiveAssemblies => PluginsManager.ActiveAssemblies;
        public Instances<IModule> Instances => PluginsManager.Instances;
        public SettingsFile<AssemblySettings> AssembliesSettingsFile => PluginsManager.AssembliesSettingsFile;

        #endregion

        public ModuleService(Settings settings, BaseManager manager)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));

            PluginsManager = new PluginsManager<IModule>(Options.CompanyName,
                (module, list) =>
                {
                    foreach (var pair in list)
                    {
                        module.Settings.Set(pair.Key, pair.Value ?? throw new InvalidOperationException("pair.Value is null"));
                    }
                },
                module => module.Settings.Select(pair => new SettingItem(pair.Key, pair.Value.Value)));
        }

        public void AddUniqueInstancesIfNeed() => SafeActions.Run(() =>
        {
            foreach (var type in PluginsManager.AvailableTypes)
            {
                if (!type.AllowMultipleInstance() &&
                    !PluginsManager.Instances.Objects.ContainsKey(type.Name))
                {
                    PluginsManager.AddInstance(type.Name, type, true);
                }
            }
        });

        public void UpdateActiveModules()
        {
            Manager.Recorder = Recorder;
            Manager.Converter = Converter;
            Manager.AlternativeConverters.Clear();
            Manager.AlternativeConverters.AddRange(AlternativeConverters);
        }

        public void RegisterHandlers(RunnerService runnerService) => SafeActions.Run(() =>
        {
            var instances = PluginsManager.Instances.Objects;

            foreach (var instance in instances)
            {
                var name = instance.Key;
                var module = instance.Value.Value;
                if (module == null || module.IsRegistered)
                {
                    continue;
                }

                module.IsRegistered = true;
                module.UniqueName = name;
                module.NewCommand += (sender, command) => runnerService.Run(command);
                module.NewCommandAsync += async (sender, args) =>
                {
                    if (args == null)
                    {
                        return;
                    }

                    using (args.GetDeferral())
                    {
                        await runnerService.RunAsync(args.Text);
                    }
                };

                module.SettingsSaved += (sender, value) => PluginsManager.SavePluginSettings(value.ShortName, value);
            }
        });

        public void AddInstancesFromAssembly(string path, Type interfaceType, Predicate<Type>? filter = null)
        {
            PluginsManager.AddInstancesFromAssembly(path, interfaceType, filter);
        }

        public Assembly Install(string originalPath)
        {
            return PluginsManager.Install(originalPath);
        }

        public void Uninstall(string name)
        {
            PluginsManager.Uninstall(name);
        }

        public void Update(string name)
        {
            PluginsManager.Update(name);
        }

        public void AddInstance(string name, Type type, bool isEnabled)
        {
            PluginsManager.AddInstance(name, type, isEnabled);
        }

        public void DeleteInstance(string name)
        {
            PluginsManager.DeleteInstance(name);
        }

        public void RenameInstance(string name, string newName)
        {
            PluginsManager.RenameInstance(name, newName);
        }

        public void SetInstanceIsEnabled(string name, bool value)
        {
            PluginsManager.SetInstanceIsEnabled(name, value);
        }

        public bool UpdatingIsNeed(string name)
        {
            return PluginsManager.UpdatingIsNeed(name);
        }

        public string[] GetCanBeUpdatedAssemblies() =>
            PluginsManager.AssembliesSettings.Keys.Where(UpdatingIsNeed).ToArray();

        public void Save()
        {
            PluginsManager.Save();
        }

        public void Load()
        {
            PluginsManager.Load();
        }

        public List<KeyValuePair<string, RuntimeObject<T>>> GetPlugins<T>() where T : class, IModule
        {
            return PluginsManager.GetPlugins<T>();
        }


        public List<KeyValuePair<string, RuntimeObject<IModule>>> GetBasePlugins<T>() where T : class, IModule
        {
            return PluginsManager.GetBasePlugins<T>();
        }

        public List<KeyValuePair<string, RuntimeObject<T>>> GetEnabledPlugins<T>() where T : class, IModule
        {
            return PluginsManager.GetEnabledPlugins<T>();
        }

        public RuntimeObject<T> GetPlugin<T>(string name) where T : class, IModule
        {
            return PluginsManager.GetPlugin<T>(name);
        }

        public void AddStaticInstance(string name, IModule obj)
        {
            PluginsManager.AddStaticInstance(name, obj);
        }

        public void Dispose()
        {
            PluginsManager.Dispose();
        }
    }
}
*/