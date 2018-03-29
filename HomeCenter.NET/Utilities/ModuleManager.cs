using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using H.NET.Core;
using H.NET.Core.Attributes;
using H.NET.Plugins;
using Newtonsoft.Json;

namespace HomeCenter.NET.Utilities
{
    public static class ModuleManager
    {
        #region Properties

        public class Item
        {
            public string Key { get; }
            public object Value { get; }

            public Item(string key, object value)
            {
                Key = key;
                Value = value;
            }
        }

        public static PluginsManager<IModule> Instance { get; } = new PluginsManager<IModule>(Options.CompanyName,
            (module, text) =>
            {
                var list = JsonConvert.DeserializeObject<List<Item>>(text);
                if (list == null)
                {
                    return;
                }

                foreach (var pair in list)
                {
                    module.Settings.CopyFrom(pair.Key, pair.Value);
                }
            }, module =>
            {
                if (module == null)
                {
                    return null;
                }

                var list = module.Settings.Select(pair => new Item(pair.Key, pair.Value.Value)).ToList();

                return JsonConvert.SerializeObject(list, Formatting.Indented);
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
