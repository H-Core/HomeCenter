using System.Windows;

namespace HomeCenter.NET.Views.Settings
{
    public partial class BoolSettingControl
    {
        #region Static Dependency Properties

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text), typeof(string), typeof(BoolSettingControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(
                nameof(IsChecked), typeof(bool?), typeof(BoolSettingControl),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        #region Properties

        public string? Text {
            get => GetValue(TextProperty).ToString();
            set => SetValue(TextProperty, value);
        }

        public bool? IsChecked {
            get => GetValue(IsCheckedProperty) as bool?;
            set => SetValue(IsCheckedProperty, value);
        }

        #endregion

        #region Constructors

        public BoolSettingControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
