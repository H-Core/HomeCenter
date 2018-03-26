using System;
using System.Windows;
using H.NET.Core;

namespace HomeCenter.NET.Windows
{
    public partial class ModuleSettingsWindow
    {
        #region Properties

        private IModule Module { get; }

        #endregion

        #region Contructors

        public ModuleSettingsWindow(IModule module)
        {
            Module = module ?? throw new ArgumentNullException(nameof(module));

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
            foreach (var pair in Module.Settings)
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

                Module.Settings[settingControl.Setting.Key] = settingControl.Setting;
            }
        }

        #endregion

    }
}
