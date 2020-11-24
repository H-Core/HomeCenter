using System;
using H.Core.Settings;

namespace HomeCenter.NET.ViewModels.Modules
{
    public class BooleanSettingViewModel : SettingViewModel
    {
        #region Constructors

        public BooleanSettingViewModel(Setting setting) : base(setting)
        {
            if (setting.Type != typeof(bool))
            {
                throw new ArgumentException(@"Incorrect setting type", nameof(setting));
            }
        }

        #endregion
    }
}
