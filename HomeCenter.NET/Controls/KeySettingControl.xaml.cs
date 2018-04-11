using System.Windows;

namespace HomeCenter.NET.Controls
{
    public partial class KeySettingControl
    {
        #region Static Dependency Properties

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text), typeof(string), typeof(KeySettingControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(
                nameof(Key), typeof(string), typeof(KeySettingControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        #region Properties

        public string Text {
            get => GetValue(TextProperty).ToString();
            set => SetValue(TextProperty, value);
        }

        public string Key {
            get => GetValue(KeyProperty).ToString();
            set => SetValue(KeyProperty, value);
        }

        #endregion

        #region Constructors

        public KeySettingControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
