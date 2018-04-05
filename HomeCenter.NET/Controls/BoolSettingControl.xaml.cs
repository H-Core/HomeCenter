using System.ComponentModel;
using System.Windows;

namespace HomeCenter.NET.Controls
{
    public partial class BoolSettingControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static void OnPropertyChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as BoolSettingControl;

            control?.OnCustomPropertyChanged(e);
        }

        private void OnCustomPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var name = e.Property.Name;
            switch (name)
            {
                case nameof(Text):
                    Label.Content = Text;
                    break;

                case nameof(IsChecked):
                    CheckBox.IsChecked = IsChecked;
                    break;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #region Static Dependency Properties

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(BoolSettingControl),
                new PropertyMetadata(string.Empty, OnPropertyChanged));

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool?), typeof(BoolSettingControl),
                new PropertyMetadata(false, OnPropertyChanged));

        #endregion

        #region Properties

        public string Text {
            get => GetValue(TextProperty).ToString();
            set => SetValue(TextProperty, value);
        }

        public bool? IsChecked {
            get => GetValue(IsCheckedProperty) as bool?;
            set => SetValue(IsCheckedProperty, value);
        }

        #endregion

        public BoolSettingControl()
        {
            InitializeComponent();
        }
    }
}
