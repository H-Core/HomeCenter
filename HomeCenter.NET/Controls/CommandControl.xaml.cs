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

        public bool NameLabelEnabled {
            get => NameLabel.Visibility == Visibility.Visible;
            set => NameLabel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool RunButtonEnabled {
            get => RunButton.Visibility == Visibility.Visible;
            set => RunButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool EditButtonEnabled
        {
            get => EditButton.Visibility == Visibility.Visible;
            set => EditButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool DeleteButtonEnabled {
            get => DeleteButton.Visibility == Visibility.Visible;
            set => DeleteButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Events

        public event EventHandler Run;
        public event EventHandler Edited;
        public event EventHandler Deleted;

        #endregion

        #region Constructors

        public CommandControl(string name, string description, string hotKey = null, bool editable = false, 
            bool run = false, bool edit = false, bool delete = false)
        {
            InitializeComponent();

            ObjectName = name;
            ObjectDescription = description;

            IsEditable = editable;

            NameLabelEnabled = name != null;
            RunButtonEnabled = run;
            EditButtonEnabled = edit;
            DeleteButtonEnabled = delete;

            HotKeyLabel.Visibility = hotKey == null ? Visibility.Collapsed : Visibility.Visible;
            HotKeyLabel.Content = hotKey ?? string.Empty;
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
