using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using H.NET.Core;
using H.NET.Core.Attributes;
using H.NET.Plugins;
using HomeCenter.NET.Controls;
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

            Properties.Settings.Default.Recorder = RecorderComboBox.SelectedItem as string;
            Properties.Settings.Default.Save();
            Startup.Set(Options.FileName, StartupCheckBox.IsChecked ?? false);

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
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

            Add(path);
        }

        #endregion

        #region Private methods

        private void Add(string path) => SafeActions.Run(() =>
        {
            ModuleManager.Instance.AddInstancesFromAssembly(path, typeof(IModule), 
                type =>
                !(type.GetCustomAttribute(typeof(AllowMultipleInstanceAttribute)) is AllowMultipleInstanceAttribute attribute) ||
                 attribute.AutoCreateInstance);

            Update();
        });

        private void Update()
        {
            UpdateAssemblies();
            UpdateAvailableTypes();
            UpdateModules();
            UpdateRecorders();
        }

        private InstanceControl CreateInstanceControl<T>(string name, RuntimeObject<T> instance) where T : class, IModule
        {
            var module = instance.Value;
            var control = new InstanceControl(name, module?.Name ?? instance.Exception?.Message ?? string.Empty)
            {
                Height = 25,
                Color = module != null ? Colors.LightGreen : Colors.Bisque,
                EnableEditing = module != null && module.Settings?.Count > 0,
                EnableEnabling = instance.Exception == null,
                ObjectIsEnabled = instance.IsEnabled
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

                ModuleManager.Instance.DeleteInstance(name);

                Update();
            };
            control.Edited += (sender, args) =>
            {
                var window = new ModuleSettingsWindow(module?.Settings);
                window.ShowDialog();
                Update();
            };
            control.EnabledChanged += enabled =>
            {
                ModuleManager.Instance.SetInstanceIsEnabled(name, enabled);

                Update();
            };

            return control;
        }

        private void UpdateModules() => SafeActions.Run(() =>
        {
            var instances = ModuleManager.Instance.Instances.Objects;

            ModulesPanel.Children.Clear();
            foreach (var pair in instances)
            {
                var control = CreateInstanceControl(pair.Key, pair.Value);
                ModulesPanel.Children.Add(control);
            }
        });

        private void UpdateAvailableTypes() => SafeActions.Run(() =>
        {
            var types = ModuleManager.Instance.AvailableTypes;

            AvailableTypesPanel.Children.Clear();
            foreach (var type in types)
            {
                var control = new ObjectControl(type.Name)
                {
                    Height = 25,
                    Color = Colors.LightGreen,
                    EnableEditing = false,
                    EnableAdding = type.GetCustomAttribute<AllowMultipleInstanceAttribute>() != null
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
        });

        private void UpdateAssemblies() => SafeActions.Run(() =>
        {
            var assemblies = ModuleManager.Instance.ActiveAssemblies;

            AssembliesPanel.Children.Clear();
            foreach (var assembly in assemblies)
            {
                var control = new ObjectControl(assembly.GetName().Name)
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
        });

        private void UpdateRecorders() => SafeActions.Run(() =>
        {
            var name = Properties.Settings.Default.Recorder;
            var availableRecorders = ModuleManager.Instance.GetEnabledPlugins<IRecorder>().Select(i => i.Key).ToList();
            if (!string.IsNullOrWhiteSpace(name) && !availableRecorders.Contains(name))
            {
                availableRecorders.Add(name);

            }
            RecorderComboBox.ItemsSource = availableRecorders;
            RecorderComboBox.SelectedItem = name;

            var allRecorders = ModuleManager.Instance.GetPlugins<IRecorder>();
            RecordersPanel.Children.Clear();
            foreach (var pair in allRecorders)
            {
                var control = CreateInstanceControl(pair.Key, pair.Value);
                RecordersPanel.Children.Add(control);
            }
        });

        #endregion
    }
}
