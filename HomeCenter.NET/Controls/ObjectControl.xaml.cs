using System;
using System.Windows;
using System.Windows.Media;

namespace HomeCenter.NET.Controls
{
    public partial class ObjectControl
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

        public bool EnableName {
            get => NameLabel.Visibility == Visibility.Visible;
            set => NameLabel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool EnableUpdating {
            get => UpdateButton.Visibility == Visibility.Visible;
            set => UpdateButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool EnableAdding {
            get => AddButton.Visibility == Visibility.Visible;
            set => AddButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool EnableEditing
        {
            get => EditButton.Visibility == Visibility.Visible;
            set => EditButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool EnableDeleting {
            get => DeleteButton.Visibility == Visibility.Visible;
            set => DeleteButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Events

        public event EventHandler Updated;
        public event EventHandler Added;
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

        public ObjectControl(string name) : this(string.Empty, name)
        {
            EnableName = false;
        }

        #endregion

        #region Event handlers

        private void UpdateButton_Click(object sender, RoutedEventArgs e) =>
            Updated?.Invoke(this, EventArgs.Empty);

        private void AddButton_Click(object sender, RoutedEventArgs e) =>
            Added?.Invoke(this, EventArgs.Empty);

        private void EditButton_Click(object sender, RoutedEventArgs e) =>
            Edited?.Invoke(this, EventArgs.Empty);

        private void DeleteButton_Click(object sender, RoutedEventArgs e) =>
            Deleted?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
