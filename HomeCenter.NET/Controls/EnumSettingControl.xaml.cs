using System.Windows;

namespace HomeCenter.NET.Controls
{
    public partial class EnumSettingControl
    {
        #region Static Dependency Properties

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text), typeof(string), typeof(EnumSettingControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        #region Properties

        public string Text {
            get => GetValue(TextProperty).ToString();
            set => SetValue(TextProperty, value);
        }

        #endregion

        #region Constructors

        public EnumSettingControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
