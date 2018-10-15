using System;
using System.Windows;
using Caliburn.Micro;
using H.NET.Core.Settings;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.ViewModels.Modules
{
    public class SettingViewModel : Screen
    {
        #region Properties

        public Setting Setting { get; }

        public object Value {
            get => Setting.Value;
            set {
                SafeActions.Run(() =>
                {
                    Setting.Value = Convert.ChangeType(value, Setting.Type);
                    NotifyOfPropertyChange(nameof(Value));
                });

                NotifyOfPropertyChange(nameof(IsValid));
            }
        }

        public string Name => $"{Setting.Key}({Setting.Type})";

        public bool IsValid => Setting.IsValid();

        #endregion

        #region Constructors

        protected SettingViewModel(Setting setting)
        {
            Setting = setting ?? throw new ArgumentNullException(nameof(setting));
        }

        #endregion

        #region Public methods

        public void ResetDefault()
        {
            // TODO: to WindowManager
            if (MessageBox.Show(
                    "Are you sure you want to reset the setting?",
                    "Reset",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.Yes)
                != MessageBoxResult.Yes)
            {
                return;
            }

            Value = Setting.DefaultValue;
        }

        #endregion
    }
}
