using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using H.NET.Core;
using H.NET.Utilities;
using HomeCenter.NET.Properties;

namespace HomeCenter.NET.Utilities
{
    public static class Options
    {
        public static string FileName => Assembly.GetExecutingAssembly().Location;
        public const string CompanyName = "HomeCenter.NET";
        public const int IpcPortToHomeCenter = 19445;
        public const int IpcPortToDeskBand = 19446;

        public static Keys RecordKey => Hook.FromString(Settings.Default.RecordKey);

        public static IRecorder Recorder => ModuleManager.Instance.GetPlugin<IRecorder>(Settings.Default.Recorder)?.Value;
        public static IConverter Converter => ModuleManager.Instance.GetPlugin<IConverter>(Settings.Default.Converter)?.Value;
        public static List<IConverter> AlternativeConverters => Settings.Default.UseAlternativeConverters
            ? ModuleManager.Instance
            .GetEnabledPlugins<IConverter>()
            .Where(pair => !string.Equals(pair.Key, Settings.Default.Converter))
            .Select(pair => pair.Value.Value)
            .ToList() : new List<IConverter>();
        public static ISynthesizer Synthesizer => ModuleManager.Instance.GetPlugin<ISynthesizer>(Settings.Default.Synthesizer)?.Value;
        public static List<IRunner> Runners => ModuleManager.Instance
            .GetEnabledPlugins<IRunner>()
            .Select(i => i.Value.Value)
            .ToList();
    }
}
