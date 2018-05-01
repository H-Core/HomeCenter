using System;
using System.Linq;
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
                    module.Settings.CopyFrom(pair.Key, pair.Value);
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

        public static void RegisterHandlers(TextDelegate outputAction, TextDelegate sayAction, TextDelegate commandAction) => SafeActions.Run(() =>
        {
            var instances = Instance.Instances.Objects.Values;

            foreach (var instance in instances)
            {
                var module = instance.Value;
                if (module == null)
                {
                    continue;
                }

                module.NewOutput += outputAction;
                module.NewSpeech += sayAction;
                module.NewCommand += commandAction;
            }
        });
    }
}
