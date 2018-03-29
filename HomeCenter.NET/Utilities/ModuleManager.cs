using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using H.NET.Core;
using H.NET.Core.Attributes;
using H.NET.Core.Settings;
using H.NET.Plugins;
using H.NET.Storages;
using Newtonsoft.Json;

namespace HomeCenter.NET.Utilities
{
    public static class ModuleManager
    {
        #region Properties

        public static PluginsManager<IModule> Instance { get; } = new PluginsManager<IModule>(Options.CompanyName,
            (module, text) =>
            {
                var list = JsonConvert.DeserializeObject<List<Setting>>(text);
                if (list == null)
                {
                    return;
                }

                foreach (var value in list)
                {
                    module.Settings.CopyFrom(value.Key, value);
                }
            }, module =>
            {
                if (module == null)
                {
                    return null;
                }

                var list = module.Settings.Select(i => i.Value).ToList();

                return JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
                {
                    ContractResolver = new GoodPropertiesOnlyResolver()
                });
            });

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
                if (!(type.GetCustomAttribute(typeof(AllowMultipleInstanceAttribute)) is AllowMultipleInstanceAttribute) &&
                    !Instance.Instances.ContainsKey(type.Name.ToLowerInvariant()))
                {
                    Instance.AddInstance(type.Name, type);
                }
            }
        });
    }
}
