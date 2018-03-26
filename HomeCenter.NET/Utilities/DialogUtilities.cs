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
    }
}
