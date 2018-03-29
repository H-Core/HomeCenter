using System;
using System.Windows;
using H.NET.Core;

namespace HomeCenter.NET.Windows
{
    public partial class ModuleSettingsWindow
    {
        #region Properties

        private ISettingsStorage Storage { get; }

        #endregion

        #region Contructors

        public ModuleSettingsWindow(ISettingsStorage storage)
        {
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));

            InitializeComponent();

            Update();
        }

        #endregion

        #region Event handlers

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion

        #region Private methods

        private void Update()
        {
            Panel.Children.Clear();
            foreach (var pair in Storage)
            {
                var control = new Controls.SettingControl(pair.Value) { Height = 25 };
                Panel.Children.Add(control);
            }
        }

        private void Save()
        {
            foreach (var control in Panel.Children)
            {
                if (!(control is Controls.SettingControl settingControl))
                {
                    continue;
                }

                Storage[settingControl.Setting.Key] = settingControl.Setting;
            }
        }

        #endregion

    }
}
