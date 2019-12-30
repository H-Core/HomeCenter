using System;
using System.Collections.Generic;
using Caliburn.Micro;
using H.NET.Core.Settings;

namespace HomeCenter.NET.ViewModels.Modules
{
    public class EnumSettingViewModel : SettingViewModel
    {
        public BindableCollection<string> Elements { get; }

        public string? SelectedElement {
            get => Value as string;
            set {
                Value = value;
                NotifyOfPropertyChange(nameof(SelectedElement));
            }
        }

        #region Constructors

        public EnumSettingViewModel(Setting setting) : base(setting)
        {
            if (!(setting.DefaultValue is IEnumerable<string>))
            {
                throw new ArgumentException(@"Incorrect setting type", nameof(setting));
            }

            var enumerable = setting.DefaultValue as string[];

            Elements = new BindableCollection<string>(enumerable);
        }

        #endregion
    }
}
