using System.IO;
using System.Windows.Forms;

namespace HomeCenter.NET.Utilities
{
    public static class DialogUtilities
    {
        public static string? OpenFileDialog(string? path = null, string? filter = null)
        {
            var dialog = new OpenFileDialog();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                dialog.Filter = filter;
            }
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                dialog.FileName = path;
            }

            return dialog.ShowDialog() == DialogResult.OK
                ? dialog.FileName
                : null;
        }

        public static string? OpenFolderDialog(string? path = null)
        {
            var dialog = new FolderBrowserDialog();
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                dialog.SelectedPath = path + Path.DirectorySeparatorChar;
            }

            return dialog.ShowDialog() == DialogResult.OK
                ? dialog.SelectedPath
                : null;
        }
    }
}
