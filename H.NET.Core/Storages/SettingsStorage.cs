﻿using System.Collections.Generic;
using System.ComponentModel;
using H.NET.Core.Settings;

namespace H.NET.Core.Storages
{
    public class SettingsStorage : Dictionary<string, Setting>, ISettingsStorage
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public new Setting this[string key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
            }
        }

        public void CopyFrom(string key, Setting setting)
        {
            if (!TryGetValue(key, out var thisSetting))
            {
                return;
            }

            thisSetting.Key = setting.Key;
            thisSetting.Value = setting.Value;
            thisSetting.DefaultValue = setting.DefaultValue;
            thisSetting.SettingType = setting.SettingType;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
        }

        public new bool Remove(string key)
        {
            var result = base.Remove(key);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));

            return result;
        }

        #endregion

        public void Load() { }
        public void Save() { }
    }
}
