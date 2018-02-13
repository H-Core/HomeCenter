using System;

namespace HomeCenter.NET.Controls
{
    public partial class ObjectControl
    {
        #region Properties

        public string ObjectName { get; set; }
        public string ObjectDescription { get; set; }

        #endregion

        #region Events

        public event EventHandler Edited;
        public event EventHandler Deleted;

        #endregion

        #region Constructors

        public ObjectControl(string name, string description)
        {
            ObjectName = name ?? throw new ArgumentNullException(nameof(name));
            ObjectDescription = description ?? throw new ArgumentNullException(nameof(description));

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void Edit_Click(object sender, System.Windows.RoutedEventArgs e) =>
            Edited?.Invoke(this, EventArgs.Empty);

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e) =>
            Deleted?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
