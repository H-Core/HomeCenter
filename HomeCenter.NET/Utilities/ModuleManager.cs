using System;
using System.Collections.Generic;
using System.Linq;
using H.NET.Core;
using H.NET.Core.Settings;
using H.NET.Plugins;
using H.NET.Storages;
using Newtonsoft.Json;

namespace HomeCenter.NET.Utilities
{
    public static class ModuleManager
    {
        #region Properties

        public static PluginsManager<IModule> Instance { get; } = new PluginsManager<IModule>("H.NET",
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
    }
}
