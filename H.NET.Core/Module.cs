using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H.NET.Core.Settings;
using H.NET.Core.Storages;
using H.NET.Core.Utilities;

namespace H.NET.Core
{
    public class Module : IModule
    {
        #region Properties

        public string Name { get; }
        public string ShortName => GetType().Name;
        public string UniqueName { get; set; }
        public bool IsRegistered { get; set; }
        public string Description { get; } = string.Empty;

        public ISettingsStorage Settings { get; } = new SettingsStorage();
        public bool IsValid() => Settings.All(entry => entry.Value.IsValid());

        protected InvariantStringDictionary<Func<object>> Variables { get; } = new InvariantStringDictionary<Func<object>>();
        public string[] GetSupportedVariables() => Variables.Keys.ToArray();

        protected void AddVariable(string key, Func<object> action) => Variables[key] = action;
        public object GetModuleVariableValue(string key) => Variables.TryGetValue(key, out var func) ? func?.Invoke() : null;

        #endregion

        #region Events

        public event EventHandler<string> NewCommand;
        public event EventHandler<TextDeferredEventArgs> NewCommandAsync;
        public event EventHandler<IModule> SettingsSaved;
        public event EventHandler<Exception> ExceptionOccurred;

        protected void OnExceptionOccurred(Exception value)
        {
            ExceptionOccurred?.Invoke(this, value);
        }

        #endregion


        #region Constructors

        protected Module()
        {
            Name = GetType().FullName;
            UniqueName = GetType().Name;

            Settings.PropertyChanged += (sender, args) =>
            {
                var key = args.PropertyName;
                if (!Settings.ContainsKey(key))
                {
                    Log($"Settings is not exists: {key}");
                }
            };
        }

        #endregion

        #region Main Methods

        public void Run(string text) => NewCommand?.Invoke(this, text);

        public void Say(string text) => Run($"say {text}");
        public void Print(string text) => Run($"print {text}");
        public void ShowSettings() => Run($"show-module-settings {UniqueName}");

        public async Task RunAsync(string text) => await NewCommandAsync.InvokeAsync(this, TextDeferredEventArgs.Create(text));

        public async Task SayAsync(string text) => await RunAsync($"say {text}");

        public void Enable() => Run($"enable-module {UniqueName}");
        public void Disable() => Run($"disable-module {UniqueName}");

        public void SaveSettings() => SettingsSaved?.Invoke(this, this);

        #endregion

        #region Private/protected methods

        #region Settings

        public ICollection<string> GetAvailableSettings()
        {
            return Settings.Keys;
        }

        public void SetSetting(string key, object value)
        {
            Settings.Set(key, value);
        }

        public object GetSetting(string key)
        {
            return Settings[key].Value;
        }

        protected void AddSetting<T>(string key, Action<T> setAction, Func<T, bool> checkFunc, T defaultValue, SettingType type = SettingType.Default)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            Settings[key] = new Setting
            {
                Key = key,
                Value = defaultValue,
                DefaultValue = defaultValue,
                SettingType = type,
                CheckFunc = o => CanConvert<T>(o) && checkFunc?.Invoke(ConvertTo<T>(o)) != false,
                SetAction = o => setAction?.Invoke(ConvertTo<T>(o))
            };
        }

        protected void AddEnumerableSetting<T>(string key, Action<T> setAction, Func<T, bool> checkFunc, T[] defaultValues)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            Settings[key] = new Setting
            {
                Key = key,
                Value = defaultValues.ElementAtOrDefault(0),
                DefaultValue = defaultValues,
                SettingType = SettingType.Enumerable,
                CheckFunc = o => CanConvert<T>(o) && checkFunc?.Invoke(ConvertTo<T>(o)) == true,
                SetAction = o => setAction?.Invoke(ConvertTo<T>(o))
            };
        }

        protected static bool IsNull(string key) => key == null;
        protected static bool NoEmpty(string key) => !string.IsNullOrEmpty(key);
        protected static bool Always<T>(T key) => true;
        protected static bool Any(object key) => true;

        protected static bool IsNull(int key) => key == 0;
        protected static bool Positive(int key) => key > 0;
        protected static bool Negative(int key) => key < 0;
        protected static bool NotNegative(int key) => key >= 0;
        protected static bool NotPositive(int key) => key <= 0;


        #endregion

        private bool CanConvert<T>(object value)
        {
            try
            {
                var unused = Convert.ChangeType(value, typeof(T));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private T ConvertTo<T>(object value) => (T)Convert.ChangeType(value, typeof(T));

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion

        #region Static methods

        public static Action<string> LogAction { get; set; }
        public static void Log(string text) => LogAction?.Invoke(text);

        public static Func<string, object> GetVariableValueGlobalFunc { get; set; }
        public static object GetVariable(string key) => GetVariableValueGlobalFunc?.Invoke(key);
        public static T GetVariable<T>(string key, T defaultValue = default(T))
        {
            return GetVariable(key) is T value ? value : defaultValue;
        }

        public static Func<string, Task<List<string>>> SearchFunc { get; set; }
        protected static async Task<List<string>> SearchInInternet(string key)
        {
            if (SearchFunc == null)
            {
                return new List<string>();
            }

            return await SearchFunc.Invoke(key);
        }

        public static async Task<List<string>> SearchInInternet(string query, int count) =>
            (await SearchInInternet(query)).Take(count).ToList();

        #endregion
    }
}
