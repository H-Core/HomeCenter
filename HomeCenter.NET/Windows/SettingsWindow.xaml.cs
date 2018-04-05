using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using H.NET.Core;
using H.NET.Core.Extensions;
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

            StartupControl.IsChecked = Startup.IsStartup(Options.FileName);

            Update();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ModuleManager.Instance.Save();

            Properties.Settings.Default.Recorder = RecorderComboBox.SelectedItem as string;
            Properties.Settings.Default.Converter = ConverterComboBox.SelectedItem as string;
            Properties.Settings.Default.Synthesizer = SynthesizerComboBox.SelectedItem as string;
            Properties.Settings.Default.Save();
            Startup.Set(Options.FileName, StartupControl.IsChecked ?? false);

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
            ModuleManager.Instance.AddInstancesFromAssembly(path, typeof(IModule), type => type.AutoCreateInstance());

            Update();
        });

        private void Update()
        {
            UpdateAssemblies();
            UpdateAvailableTypes();
            UpdateModules();
            UpdateRecorders();
            UpdateConverters();
            UpdateSynthesizers();
            UpdateRunners();
            UpdateNotifiers();
        }

        private void UpdateModules() => SafeActions.Run(() =>
        {
            var instances = ModuleManager.Instance.Instances.Objects;
            var controls = instances.Select(pair => CreateInstanceControl(pair.Key, pair.Value, Update));

            UpdatePanel(ModulesPanel, controls);
        });

        private void UpdateAvailableTypes() => SafeActions.Run(() =>
        {
            var types = ModuleManager.Instance.AvailableTypes;
            var controls = types.Select(type =>
            {
                var control = new ObjectControl(type.Name)
                {
                    Height = 25,
                    Color = Colors.LightGreen,
                    EnableEditing = false,
                    EnableAdding = type.AllowMultipleInstance()
                };
                control.Deleted += (sender, args) =>
                {
                    ModuleManager.Instance.Deinstall(type);
                    Update();
                };
                control.Added += (sender, args) =>
                {
                    ModuleManager.Instance.AddInstance($"{type.Name}_{new Random().Next()}", type, false);

                    Update();
                };

                return control;
            });

            UpdatePanel(AvailableTypesPanel, controls);
        });

        private void UpdateAssemblies() => SafeActions.Run(() =>
        {
            var assemblies = ModuleManager.Instance.ActiveAssemblies;
            var controls = assemblies.Select(assembly =>
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

                return control;
            });

            UpdatePanel(AssembliesPanel, controls);
        });

        private void UpdateRecorders() => SafeActions.Run(() =>
        {
            UpdateComboBox<IRecorder>(RecorderComboBox, Properties.Settings.Default.Recorder);
            UpdatePanel<IRecorder>(RecordersPanel, Update);
        });

        private void UpdateConverters() => SafeActions.Run(() =>
        {
            UpdateComboBox<IConverter>(ConverterComboBox, Properties.Settings.Default.Converter);
            UpdatePanel<IConverter>(ConvertersPanel, Update);
        });

        private void UpdateSynthesizers() => SafeActions.Run(() =>
        {
            UpdateComboBox<ISynthesizer>(SynthesizerComboBox, Properties.Settings.Default.Synthesizer);
            UpdatePanel<ISynthesizer>(SynthesizersPanel, Update);
        });

        private void UpdateRunners() => SafeActions.Run(() =>
        {
            UpdatePanel<IRunner>(RunnersPanel, Update);
        });

        private void UpdateNotifiers() => SafeActions.Run(() =>
        {
            UpdatePanel<INotifier>(NotifiersPanel, Update);
        });

        #endregion

        #region Private static methods

        private static void UpdatePanel(Panel panel, IEnumerable<Control> controls)
        {
            panel.Children.Clear();
            foreach (var control in controls)
            {
                panel.Children.Add(control);
            }
        }

        private static InstanceControl CreateInstanceControl<T>(string name, RuntimeObject<T> instance, Action updateAction) where T : class, IModule
        {
            var module = instance.Value;
            var control = new InstanceControl(name, module?.Name ?? instance.Exception?.Message ?? string.Empty)
            {
                Height = 25,
                Color = module != null ? Colors.LightGreen : Colors.Bisque,
                EnableEditing = module != null && module.Settings?.Count > 0,
                EnableEnabling = instance.Exception == null,
                ObjectIsEnabled = instance.IsEnabled,
                EnableRenaming = instance.Type?.AllowMultipleInstance() ?? false
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

                updateAction?.Invoke();
            };
            control.Renamed += (sender, args) =>
            {
                var oldName = control.ObjectName;
                var newName = RenameWindow.Rename(oldName);
                if (string.IsNullOrWhiteSpace(newName) ||
                    string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                ModuleManager.Instance.RenameInstance(oldName, newName);
                control.ObjectName = newName;

                updateAction?.Invoke();
            };
            control.Edited += (sender, args) =>
            {
                var window = new ModuleSettingsWindow(module?.Settings);
                window.ShowDialog();

                updateAction?.Invoke();
            };
            control.EnabledChanged += enabled =>
            {
                ModuleManager.Instance.SetInstanceIsEnabled(name, enabled);

                updateAction?.Invoke();
            };

            return control;
        }

        private static void UpdatePanel<T>(StackPanel panel, Action updateAction) where T : class, IModule
        {
            var plugins = ModuleManager.Instance.GetPlugins<T>();
            var controls = plugins.Select(pair => CreateInstanceControl(pair.Key, pair.Value, updateAction));

            UpdatePanel(panel, controls);
        }

        private static void UpdateComboBox<T>(Selector selector, string value) where T : class, IModule
        {
            var plugins = ModuleManager.Instance.GetEnabledPlugins<T>().Select(i => i.Key).ToList();
            if (!string.IsNullOrWhiteSpace(value) && !plugins.Contains(value))
            {
                plugins.Add(value);

            }

            selector.ItemsSource = plugins;
            selector.SelectedItem = value;
        }

        #endregion
    }
}
