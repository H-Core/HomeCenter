using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using H.NET.Core;
using H.NET.Core.Extensions;
using H.NET.Plugins;
using H.NET.Utilities;
using HomeCenter.NET.Controls;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Windows
{
    public partial class SettingsWindow
    {
        #region Constructors

        public SettingsWindow() => InitializeComponent();

        #endregion

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartupControl.IsChecked = Startup.IsStartup(Options.FilePath);

            Update();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ReloadPluginsButton_Click(object sender, RoutedEventArgs e)
        {
            //ModuleManager.Instance.Load();

            Update();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var path = DialogUtilities.OpenFileDialog(filter: @"DLL files (*.dll) |*.dll");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            Add(path);
        }

        private void AddIgnoredButton_OnClick(object sender, RoutedEventArgs e)
        {
            var path = DialogUtilities.OpenFileDialog(filter: @"EXE files (*.exe) |*.exe");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            Options.HookIgnoredApps = Options.HookIgnoredApps.Concat(new[] { path }).ToList();

            UpdateIgnored();
        }

        #endregion

        #region Private methods

        private void Save()
        {
            ModuleManager.Instance.Save();

            Properties.Settings.Default.Recorder = RecorderControl.ComboBox.SelectedItem as string;
            Properties.Settings.Default.Converter = ConverterControl.ComboBox.SelectedItem as string;
            Properties.Settings.Default.Synthesizer = SynthesizerControl.ComboBox.SelectedItem as string;
            Properties.Settings.Default.Searcher = SearcherControl.ComboBox.SelectedItem as string;
            Properties.Settings.Default.Save();
            Startup.Set(Options.FilePath, StartupControl.IsChecked ?? false);
        }

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
            UpdateSearchers();
            UpdateRunners();
            UpdateNotifiers();
            UpdateIgnored();
        }

        private void UpdateModules() => SafeActions.Run(() =>
        {
            var instances = ModuleManager.Instance.Instances.Objects;
            var controls = instances.Select(pair => CreateInstanceControl(pair.Key, pair.Value, Update));

            ModulesPanel.Update(controls);
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
                    EnableAdding = type.AllowMultipleInstance(),
                    EnableUpdating = false
                };
                control.Deleted += (sender, args) =>
                {
                    ModuleManager.Instance.Uninstall(type);
                    Update();
                };
                control.Added += (sender, args) =>
                {
                    ModuleManager.Instance.AddInstance($"{type.Name}_{new Random().Next()}", type, false);

                    Update();
                };

                return control;
            });

            AvailableTypesPanel.Update(controls);
        });

        private void UpdateAssemblies() => SafeActions.Run(() =>
        {
            var assemblies = ModuleManager.Instance.ActiveAssemblies;
            var controls = assemblies.Select(pair =>
            {
                var name = pair.Key;
                var control = new ObjectControl(name)
                {
                    Height = 25,
                    Color = Colors.LightGreen,
                    EnableEditing = false,
                    EnableAdding = false,
                    EnableUpdating = ModuleManager.Instance.UpdatingIsNeed(name)
                };
                control.Deleted += (sender, args) =>
                {
                    ModuleManager.Instance.Uninstall(name);
                    Update();
                };
                control.Updated += (sender, args) =>
                {
                    ModuleManager.Instance.Update(name);
                    Update();
                };

                return control;
            });

            AssembliesPanel.Update(controls);
        });

        private void UpdateRecorders() => SafeActions.Run(() =>
        {
            UpdateComboBox<IRecorder>(RecorderControl.ComboBox, Properties.Settings.Default.Recorder);
            UpdatePanel<IRecorder>(RecordersPanel, Update);
        });

        private void UpdateConverters() => SafeActions.Run(() =>
        {
            UpdateComboBox<IConverter>(ConverterControl.ComboBox, Properties.Settings.Default.Converter);
            UpdatePanel<IConverter>(ConvertersPanel, Update);
        });

        private void UpdateSynthesizers() => SafeActions.Run(() =>
        {
            UpdateComboBox<ISynthesizer>(SynthesizerControl.ComboBox, Properties.Settings.Default.Synthesizer);
            UpdatePanel<ISynthesizer>(SynthesizersPanel, Update);
        });

        private void UpdateSearchers() => SafeActions.Run(() =>
        {
            UpdateComboBox<ISearcher>(SearcherControl.ComboBox, Properties.Settings.Default.Searcher);
            UpdatePanel<ISearcher>(SearchersPanel, Update);
        });

        private void UpdateRunners() => SafeActions.Run(() =>
        {
            UpdatePanel<IRunner>(RunnersPanel, Update);
        });

        private void UpdateNotifiers() => SafeActions.Run(() =>
        {
            UpdatePanel<INotifier>(NotifiersPanel, Update);
        });

        private void UpdateIgnored() => SafeActions.Run(() =>
        {
            var controls = Options.HookIgnoredApps.Select(app =>
            {
                var control = new ObjectControl(app)
                {
                    Height = 25,
                    Color = Colors.LightGreen,
                    EnableEditing = false,
                    EnableAdding = false,
                    EnableUpdating = false
                };
                control.Deleted += (sender, args) =>
                {
                    Options.HookIgnoredApps = Options.HookIgnoredApps.Except(new[] {app}).ToList();

                    Update();
                };

                return control;
            });

            HookIgnoredPanel.Update(controls);
        });

        #endregion

        #region Private static methods

        private static InstanceControl CreateInstanceControl<T>(string name, RuntimeObject<T> instance, Action updateAction) where T : class, IModule
        {
            var module = instance.Value;
            var deletingAllowed = instance.Exception != null || (instance.Type?.AllowMultipleInstance() ?? false);
            var control = new InstanceControl(name, module?.Name ?? instance.Exception?.Message ?? string.Empty)
            {
                Height = 25,
                Color = module != null ? Colors.LightGreen : Colors.Bisque,
                EnableEditing = module != null && module.Settings?.Count > 0,
                EnableEnabling = !instance.IsStatic && instance.Exception == null,
                ObjectIsEnabled = instance.IsEnabled,
                EnableRenaming = !instance.IsStatic && (instance.Type?.AllowMultipleInstance() ?? false),
                EnableDeleting = !instance.IsStatic && deletingAllowed
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
                var window = new ModuleSettingsWindow(module);
                window.ShowDialog();

                updateAction?.Invoke();
            };
            control.EnabledChanged += enabled =>
            {
                ModuleManager.Instance.SetInstanceIsEnabled(name, enabled);
                ModuleManager.RegisterHandlers();

                updateAction?.Invoke();
            };

            return control;
        }

        private static void UpdatePanel<T>(StackPanel panel, Action updateAction) where T : class, IModule
        {
            var plugins = ModuleManager.Instance.GetPlugins<T>();
            var controls = plugins.Select(pair => CreateInstanceControl(pair.Key, pair.Value, updateAction));

            panel.Update(controls);
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
