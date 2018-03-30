using System.IO;
using Ookii.Dialogs.Wpf;

namespace HomeCenter.NET.Utilities
{
    public static class DialogUtilities
    {
        public static string OpenFileDialog(string path = null)
        {
            var dialog = new VistaOpenFileDialog();
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                dialog.FileName = path;
            }

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public static string OpenFolderDialog(string path = null)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                dialog.SelectedPath = path + Path.DirectorySeparatorChar;
            }

            return dialog.ShowDialog() == true ? dialog.SelectedPath : null;
        }
    }
}
