using System.Timers;
using System.Windows;

namespace HomeCenter.NET.Windows
{
    public partial class PopUpWindow
    {
        #region Static methods

        private static PopUpWindow Instance { get; set; }

        public static void Show(string text, int delay)
        {
            Instance = Instance ?? new PopUpWindow();

            Instance.Message = text;
            Instance.Timeout = delay;

            Instance.Show();
        }

        #endregion

        #region Properties

        private string Message
        {
            set => TextControl.Content = value;
        }

        private int Timeout { get; set; } = int.MaxValue;

        private Timer Timer { get; set; } = new Timer(100);

        #endregion

        #region Constructors

        private PopUpWindow()
        {
            InitializeComponent();

            MaxWidth = SystemParameters.WorkArea.Width / 4;
            MaxHeight = SystemParameters.WorkArea.Height / 2;

            Timer.Elapsed += (sender, args) =>
            {
                Timeout -= 100;
                if (Timeout > 0)
                {
                    return;
                }

                Timer?.Dispose();
                Timer = null;

                Dispatcher.Invoke(Close);
                Instance = null;
            };
        }

        #endregion

        #region Event Handlers

        private void Window_Loaded(object sender, RoutedEventArgs e) => Timer.Start();

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Left = SystemParameters.WorkArea.Width - ActualWidth;
            Top = SystemParameters.WorkArea.Height - ActualHeight;
        }

        #endregion

    }
}
