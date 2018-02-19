using System;

namespace HomeCenter.NET.Controls
{
    public partial class TextControl
    {
        #region Properties

        public string Text { get; set; }

        #endregion

        #region Events

        public event EventHandler Deleted;

        #endregion

        #region Constructors

        public TextControl(string text)
        {
            Text = text ?? string.Empty;

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e) =>
            Deleted?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
