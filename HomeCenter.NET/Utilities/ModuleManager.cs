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
                    !Instance.Instances.Objects.ContainsKey(type.Name))
                {
                    Instance.AddInstance(type.Name, type);
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
