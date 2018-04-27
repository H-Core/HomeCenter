using System.Timers;

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

        private string Message {
            set => TextBlock.Text = value;
        }

        private int Timeout { get; set; } = int.MaxValue;

        private Timer Timer { get; set; } = new Timer(100);

        #endregion

        #region Constructors

        private PopUpWindow()
        {
            InitializeComponent();

            Left = System.Windows.SystemParameters.WorkArea.Width - Width;
            Top = System.Windows.SystemParameters.WorkArea.Height - Height;

            Timer.Start();
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
    }
}
