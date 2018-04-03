using System.ComponentModel;

namespace HomeCenter.NET.Windows
{
    public partial class RenameWindow : INotifyPropertyChanged
    {
        #region Static methods

        public static string Rename(string oldName)
        {
            var window = new RenameWindow(oldName, oldName);
            if (window.ShowDialog() != true)
            {
                return null;
            }

            return window.NewName;
        }

        #endregion

        #region Properties

        private string _oldName;
        private string _newName;

        public string OldName {
            get => _oldName;
            set {
                _oldName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OldName)));
            }
        }

        public string NewName {
            get => _newName;
            set {
                _newName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewName)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Contructors

        public RenameWindow(string oldName, string newName = null)
        {
            InitializeComponent();

            OldName = oldName ?? string.Empty;
            NewName = newName ?? string.Empty;
        }

        #endregion

        #region Event Handlers

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion
    }
}
