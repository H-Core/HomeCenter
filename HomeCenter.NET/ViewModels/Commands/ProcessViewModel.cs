using System;
using System.Windows.Media;
using HomeCenter.NET.Runners;

namespace HomeCenter.NET.ViewModels.Commands
{
    public class ProcessViewModel : CommandViewModel
    {
        #region Properties

        public GlobalRunner.Process Process { get; }
        public Color Color { get; } // TODO: Remove Windows dependency

        #endregion

        #region Constructors

        public ProcessViewModel(GlobalRunner.Process process) : 
            base(null, process?.Name ?? string.Empty,
                run: true, delete: !process?.IsCompleted ?? false)
        {
            Process = process ?? throw new ArgumentNullException(nameof(process));

            Color = process.IsCanceled ? Colors.Gold : process.IsCompleted ? Colors.LightGreen : Colors.Lavender;

        }

        #endregion
    }
}
