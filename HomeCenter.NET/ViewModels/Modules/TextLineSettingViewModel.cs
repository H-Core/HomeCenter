using System;
using H.NET.Core.Settings;

namespace HomeCenter.NET.ViewModels.Modules
{
    public class TextLineSettingViewModel : SettingViewModel
    {
        #region Constructors

        public TextLineSettingViewModel(Setting setting) : base(setting)
        {
            if (setting.Type != typeof(string))
            {
                throw new ArgumentException(@"Incorrect setting type", nameof(setting));
            }
        }

        #endregion
    }
}
