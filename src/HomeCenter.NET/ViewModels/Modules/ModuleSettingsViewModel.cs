using System;
using System.Linq;
using Caliburn.Micro;
using H.NET.Core;
using H.NET.Core.Settings;

namespace HomeCenter.NET.ViewModels.Modules
{
    public class ModuleSettingsViewModel : SaveCancelViewModel
    {
        #region Properties

        private IModule Module { get; }
        public BindableCollection<SettingViewModel> Settings { get; }

        #endregion

        #region Constructors

        public ModuleSettingsViewModel(IModule module)
        {
            Module = module ?? throw new ArgumentNullException(nameof(module));

            Settings = new BindableCollection<SettingViewModel>(
                Module.Settings.Select(i => CreateSettingViewModel(i.Value)));

            SaveAction = () => Module.SaveSettings();
        }

        protected static SettingViewModel CreateSettingViewModel(Setting setting)
        {
            switch (setting.SettingType)
            {
                case SettingType.Default:
                    if (setting.Type == typeof(string))
                    {
                        return new TextLineSettingViewModel(setting);
                    }
                    if (setting.Type == typeof(int))
                    {
                        return new TextLineSettingViewModel(setting);
                    }
                    if (setting.Type == typeof(bool))
                    {
                        return new BooleanSettingViewModel(setting);
                    }

                    break;

                case SettingType.Path:
                case SettingType.Folder:
                    return new BrowseSettingViewModel(setting);

                case SettingType.Enumerable:
                    return new EnumSettingViewModel(setting);
            }

            throw new NotImplementedException();
        }

        #endregion
    }
}
