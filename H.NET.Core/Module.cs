using System;
using System.ComponentModel;
using System.Linq;
using H.NET.Core.Settings;
using H.NET.Core.Storages;

namespace H.NET.Core
{
    public class Module : IModule
    {
        #region Properties

        public string Name { get; }
        public string Description { get; } = string.Empty;

        public ISettingsStorage Settings { get; } = new SettingsStorage();
        public bool IsValid() => Settings.All(entry => entry.Value.IsValid());

        #endregion

        #region Constructors

        protected Module()
        {
            Name = GetType().FullName;

            Settings.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Private/protected methods

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var key = args.PropertyName;
            if (!Settings.ContainsKey(key))
            {
                Log($"Settings is not exists: {key}");
                return;
            }

            var setting = Settings[key];

            //if (!setting.IsValid())
            //{
            //    Log($"Warning: {setting.Key}: {setting.Value} is not valid");
            //    //return;
            //}

            setting.Set();
        }

        protected void AddSetting<T>(string key, Action<T> action, Func<T, bool> checkFunc, T defaultValue, SettingType type = SettingType.Default)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            Settings[key] = new Setting
            {
                Key = key,
                Value = defaultValue,
                DefaultValue = defaultValue,
                SettingType = type,
                CheckFunc = o => CanConvert<T>(o) && checkFunc?.Invoke(ConvertTo<T>(o)) == true, 
                SetAction = o => action?.Invoke(ConvertTo<T>(o))
            };
        }

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

        #endregion
    }
}
