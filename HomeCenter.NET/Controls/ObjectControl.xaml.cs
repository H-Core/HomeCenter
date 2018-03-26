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

        public bool EnableAdding {
            get => AddButtton.Visibility == Visibility.Visible;
            set => AddButtton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool EnableEditing
        {
            get => EditButtton.Visibility == Visibility.Visible;
            set => EditButtton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool EnableDeleting {
            get => DeleteButtton.Visibility == Visibility.Visible;
            set => DeleteButtton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Events

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

        private void AddButton_Click(object sender, RoutedEventArgs e) =>
            Added?.Invoke(this, EventArgs.Empty);

        private void EditButton_Click(object sender, RoutedEventArgs e) =>
            Edited?.Invoke(this, EventArgs.Empty);

        private void DeleteButton_Click(object sender, RoutedEventArgs e) =>
            Deleted?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
