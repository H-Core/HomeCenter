using System;
using System.Collections.Generic;
using System.Linq;
using H.NET.Core;
using H.NET.Core.Extensions;
using H.NET.Core.Managers;
using H.NET.Utilities.Plugins;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Services
{
    public class ModuleService : PluginsManager<IModule>
    {
        #region Properties

        public Settings Settings { get; }
        public BaseManager Manager { get; }

        public IRecorder? Recorder => GetPlugin<IRecorder>(Settings.Recorder)?.Value;
        public ISearcher? Searcher => GetPlugin<ISearcher>(Settings.Searcher)?.Value;
        public IConverter? Converter => GetPlugin<IConverter>(Settings.Converter)?.Value;
        public ISynthesizer? Synthesizer => GetPlugin<ISynthesizer>(Settings.Synthesizer)?.Value;

        public List<IConverter> AlternativeConverters => Settings.UseAlternativeConverters
            ? GetEnabledPlugins<IConverter>()
                  .Where(pair => !string.Equals(pair.Key, Settings.Converter))
                  .Select(pair => pair.Value.Value)
                  .ToList() 
            : new List<IConverter>();

        public List<IRunner> Runners => GetEnabledPlugins<IRunner>().Select(i => i.Value.Value).ToList();

        public List<IModule> Modules => GetEnabledPlugins<IModule>().Select(i => i.Value.Value).ToList();

        #endregion

        public ModuleService(Settings settings, BaseManager manager) : base(
            Options.CompanyName,
            (module, list) =>
            {
                foreach (var pair in list)
                {
                    module.Settings.Set(pair.Key, pair.Value);
                }
            }, 
            module => module.Settings.Select(pair => new SettingItem(pair.Key, pair.Value.Value)))
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public void AddUniqueInstancesIfNeed() => SafeActions.Run(() =>
        {
            foreach (var type in AvailableTypes)
            {
                if (!type.AllowMultipleInstance() &&
                    !Instances.Objects.ContainsKey(type.Name))
                {
                    AddInstance(type.Name, type, true);
                }
            }
        });

        public void UpdateActiveModules()
        {
            Manager.Recorder = Recorder;
            Manager.Converter = Converter;
            Manager.AlternativeConverters = AlternativeConverters;
        }

        public void RegisterHandlers(RunnerService runnerService) => SafeActions.Run(() =>
        {
            var instances = Instances.Objects;

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
                module.NewCommand += runnerService.Run;
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

                module.SettingsSaved += o => SavePluginSettings(o.ShortName, o);
            }
        });
    }
}
