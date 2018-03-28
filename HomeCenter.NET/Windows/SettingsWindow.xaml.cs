using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using H.NET.Core;
using H.NET.Core.Attributes;
using H.NET.Plugins.Extensions;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Windows
{
    public partial class SettingsWindow
    {
        #region Properties

        public SettingsWindow()
        {
            InitializeComponent();

            StartupCheckBox.IsChecked = Startup.IsStartup(Options.FileName);

            Update();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ModuleManager.Instance.Save();

            Properties.Settings.Default.Save();
            Startup.Set(Options.FileName, StartupCheckBox.IsChecked ?? false);

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ReloadPluginsButton_Click(object sender, RoutedEventArgs e)
        {
            ModuleManager.Instance.Load();

            Update();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var path = DialogUtilities.OpenFileDialog();
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            try
            {
                var assembly = ModuleManager.Instance.InstallAndGet(path);
                var types = assembly.GetTypesOfInterface<IModule>();
                foreach (var type in types)
                {
                    if (type.GetCustomAttribute(typeof(DisableAutoCreateInstanceAttribute)) != null)
                    {
                        continue;
                    }

                    ModuleManager.Instance.AddInstance(type.Name, type);
                }

                Update();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Private methods

        private void Update()
        {
            // Assemblies

            var assemblies = ModuleManager.Instance.ActiveAssemblies;

            AssembliesPanel.Children.Clear();
            foreach (var assembly in assemblies)
            {
                var control = new Controls.ObjectControl(assembly.GetName().Name)
                {
                    Height = 25,
                    Color = Colors.LightGreen,
                    EnableEditing = false,
                    EnableAdding = false
                };
                control.Deleted += (sender, args) =>
                {
                    ModuleManager.Instance.Deinstall(assembly);
                    Update();
                };
                AssembliesPanel.Children.Add(control);
            }

            // Available types

            var types = ModuleManager.Instance.AvailableTypes;

            AvailableTypesPanel.Children.Clear();
            foreach (var type in types)
            {
                var control = new Controls.ObjectControl(type.Name)
                {
                    Height = 25,
                    Color = Colors.LightGreen,
                    EnableEditing = false,
                    EnableAdding = type.GetCustomAttribute<DisableAutoCreateInstanceAttribute>() != null
                };
                control.Deleted += (sender, args) =>
                {
                    ModuleManager.Instance.Deinstall(type);
                    Update();
                };
                control.Added += (sender, args) =>
                {
                    ModuleManager.Instance.AddInstance($"{type.Name}_{new Random().Next()}", type);

                    Update();
                };
                AvailableTypesPanel.Children.Add(control);
            }

            // Modules

            var modules = ModuleManager.Instance.ActivePlugins;

            ModulesPanel.Children.Clear();
            foreach (var pair in modules)
            {
                var module = pair.Value.Value;
                var control = new Controls.InstanceControl(pair.Key, module?.Name ?? pair.Value.Exception?.Message ?? string.Empty)
                {
                    Height = 25,
                    Color = module != null ? Colors.LightGreen : Colors.Bisque,
                    EnableEditing = module != null,
                    EnableEnabling = pair.Value.Exception == null,
                    ObjectIsEnabled = pair.Value.IsEnabled
                };
                control.Deleted += (sender, args) =>
                {
                    var result = MessageBox.Show(
                        "Are you sure that you want to delete this instance of module?", "Warning",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }

                    ModuleManager.Instance.DeleteInstance(pair.Key);

                    Update();
                };
                control.Edited += (sender, args) =>
                {
                    var window = new ModuleSettingsWindow(module);
                    window.ShowDialog();
                    Update();
                };
                control.EnabledChanged += enabled =>
                {
                    ModuleManager.Instance.SetInstanceIsEnabled(pair.Key, enabled);

                    Update();
                };
                ModulesPanel.Children.Add(control);
            }
        }

        #endregion
    }
}
