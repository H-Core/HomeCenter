using System;
using System.Linq;
using System.Threading.Tasks;
using H.NET.Core;
using H.NET.Core.Extensions;
using H.NET.Plugins;

namespace HomeCenter.NET.Utilities
{
    public static class ModuleManager
    {
        #region Properties

        public static PluginsManager<IModule> Instance { get; } = new PluginsManager<IModule>(Options.CompanyName,
            (module, list) =>
            {
                foreach (var pair in list)
                {
                    module.Settings.Set(pair.Key, pair.Value);
                }
            }, module => module.Settings.Select(pair => new SettingItem(pair.Key, pair.Value.Value)));

        #endregion

        static ModuleManager()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Instance.Dispose();
        }

        public static void AddUniqueInstancesIfNeed() => SafeActions.Run(() =>
        {
            var types = Instance.AvailableTypes;
            foreach (var type in types)
            {
                if (!type.AllowMultipleInstance() &&
                    !Instance.Instances.Objects.ContainsKey(type.Name))
                {
                    Instance.AddInstance(type.Name, type, true);
                }
            }
        });

        public static void RegisterHandlers(TextDelegate commandAction, Func<string, Task> asyncCommandFunc) => SafeActions.Run(() =>
        {
            var instances = Instance.Instances.Objects.Values;

            foreach (var instance in instances)
            {
                var module = instance.Value;
                if (module == null)
                {
                    continue;
                }

                module.NewCommand += commandAction;
                module.NewCommandAsync += async (sender, args) =>
                {
                    if (asyncCommandFunc == null || args == null)
                    {
                        return;
                    }

                    using (args.GetDeferral())
                    {
                        await asyncCommandFunc(args.Text);
                    }
                };

                module.SettingsSaved += o => Instance.SavePluginSettings(o.ShortName, o);
            }
        });
    }
}
