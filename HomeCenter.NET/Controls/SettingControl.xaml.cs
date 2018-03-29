using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using H.NET.Core.Settings;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Controls
{
    public partial class SettingControl : INotifyPropertyChanged
    {
        #region Properties

        public Setting Setting { get; }

        public object Value {
            get => Setting.Value;
            set {
                try
                {
                    Setting.Value = Convert.ChangeType(value, Setting.Type);
                    OnPropertyChanged(nameof(Value));
                }
                catch (Exception exception)
                {
                    SafeActions.ShowException(exception);
                }

                UpdateColor();
            }
        }

        public string KeyName => $"{Setting.Key}({Setting.Type})";

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion

        #region Constructors

        public SettingControl(Setting setting)
        {
            Setting = setting ?? throw new ArgumentNullException(nameof(setting));

            InitializeComponent();

            switch (Setting.SettingType)
            {
                case SettingType.Default:
                    break;
                case SettingType.Path:
                    BrowseButton.Visibility = Visibility.Visible;
                    break;
                case SettingType.Enumerable:
                    if (Setting.DefaultValue is IEnumerable enumerable)
                    {
                        TextBox.Visibility = Visibility.Hidden;
                        ComboBoxBorder.Visibility = Visibility.Visible;
                        ComboBox.ItemsSource = enumerable;

                        ComboBox.SelectedItem = Setting.Value;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateColor();
        }

        public void UpdateColor()
        {
            var isValid = Setting.IsValid();
            TextBox.Background = new SolidColorBrush(isValid ? Colors.LightGreen : Colors.Bisque);
            ComboBoxBorder.BorderBrush = new SolidColorBrush(isValid ? Colors.LightGreen : Colors.Red);
        }

        #endregion

        #region Event handlers

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var text = Value as string;
            var path = DialogUtilities.OpenFileDialog(text);
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            Value = path;
        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Value = ComboBox.SelectedItem;
        }

        #endregion
    }
}
