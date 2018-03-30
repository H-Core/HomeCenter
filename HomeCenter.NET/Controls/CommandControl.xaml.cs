using System;
using System.Windows;

namespace HomeCenter.NET.Controls
{
    public partial class CommandControl
    {
        #region Properties

        public string ObjectName { get => NameLabel.Content as string; set => NameLabel.Content = value; }

        public string ObjectDescription
        {
            get => IsEditable ? DescriptionTextBox.Text : DescriptionLabel.Content as string;
            set
            {
                DescriptionLabel.Content = value;
                DescriptionTextBox.Text = value;
            }
        }

        public bool IsEditable
        {
            get => DescriptionTextBox.Visibility == Visibility.Visible;
            set
            {
                DescriptionTextBox.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                DescriptionLabel.Visibility = !value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        #endregion

        #region Events

        public event EventHandler Run;
        public event EventHandler Edited;
        public event EventHandler Deleted;

        #endregion

        #region Constructors

        public CommandControl(string name, string description, bool editable = false)
        {
            InitializeComponent();

            ObjectName = name;
            ObjectDescription = description;
            IsEditable = editable;
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
