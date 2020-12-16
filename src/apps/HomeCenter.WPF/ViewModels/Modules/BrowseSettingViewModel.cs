using System;
using H.Core.Settings;
using HomeCenter.NET.Utilities;
// ReSharper disable UnusedMember.Global

namespace HomeCenter.NET.ViewModels.Modules
{
    public class BrowseSettingViewModel : SettingViewModel
    {
        #region Constructors

        public BrowseSettingViewModel(Setting setting) : base(setting)
        {
            if (setting.Type != typeof(string))
            {
                throw new ArgumentException(@"Incorrect setting type", nameof(setting));
            }
        }

        #endregion

        #region Public methods

        public void Browse()
        {
            // TODO: replace with WindowManager
            var text = Value as string;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var path = Setting.SettingType == SettingType.Path
                ? DialogUtilities.OpenFileDialog(text)
                : DialogUtilities.OpenFolderDialog(text);
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            Value = path;
        }

        #endregion
    }
}
