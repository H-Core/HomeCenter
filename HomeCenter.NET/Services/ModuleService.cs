using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H.NET.Core;
using H.NET.Core.Extensions;
using H.NET.Plugins;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Services
{
    public class ModuleService : PluginsManager<IModule>
    {
        #region Properties

        public Settings Settings { get; }

        public TextDelegate RunAction { get; set; }
        public Func<string, Task> RunAsyncFunc { get; set; }

        public IRecorder Recorder => GetPlugin<IRecorder>(Settings.Recorder)?.Value;
        public ISearcher Searcher => GetPlugin<ISearcher>(Settings.Searcher)?.Value;
        public IConverter Converter => GetPlugin<IConverter>(Settings.Converter)?.Value;
        public List<IConverter> AlternativeConverters => Settings.UseAlternativeConverters
            ? GetEnabledPlugins<IConverter>()
                  .Where(pair => !string.Equals(pair.Key, Settings.Converter))
                  .Select(pair => pair.Value.Value)
                  .ToList() : new List<IConverter>();
        public ISynthesizer Synthesizer => GetPlugin<ISynthesizer>(Settings.Synthesizer)?.Value;
        public List<IRunner> Runners => GetEnabledPlugins<IRunner>().Select(i => i.Value.Value).ToList();

        #endregion

        public ModuleService(Settings settings) : base(
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

        public void RegisterHandlers() => SafeActions.Run(() =>
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
                module.NewCommand += RunAction;
                module.NewCommandAsync += async (sender, args) =>
                {
                    if (RunAsyncFunc == null || args == null)
                    {
                        return;
                    }

                    using (args.GetDeferral())
                    {
                        await RunAsyncFunc(args.Text);
                    }
                };

                module.SettingsSaved += o => SavePluginSettings(o.ShortName, o);
            }
        });
    }
}
