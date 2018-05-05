using System;
using System.Windows;
using H.NET.Core;
using HomeCenter.NET.Utilities;

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
        }

        #endregion

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e) => Update();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
            
            this.SetDialogResultAndClose(true);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.SetDialogResultAndClose(false);
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

            Module.SaveSettings();
        }

        #endregion
    }
}
