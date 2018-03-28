using System;
using System.Windows;
using System.Windows.Media;

namespace HomeCenter.NET.Controls
{
    public partial class CommandControl
    {
        #region Properties

        public string ObjectName { get; set; }
        public string ObjectDescription { get; set; }

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                Background = new SolidColorBrush(value);
            }
        }

        #endregion

        #region Events

        public event EventHandler Run;
        public event EventHandler Edited;
        public event EventHandler Deleted;

        #endregion

        #region Constructors

        public CommandControl(string name, string description)
        {
            ObjectName = name ?? throw new ArgumentNullException(nameof(name));
            ObjectDescription = description ?? throw new ArgumentNullException(nameof(description));

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void RunButton_Click(object sender, RoutedEventArgs e) =>
            Run?.Invoke(this, EventArgs.Empty);

        private void EditButton_Click(object sender, RoutedEventArgs e) =>
            Edited?.Invoke(this, EventArgs.Empty);

        private void DeleteButton_Click(object sender, RoutedEventArgs e) =>
            Deleted?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
