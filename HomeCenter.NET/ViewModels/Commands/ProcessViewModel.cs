using System;
using System.Windows.Media;
using HomeCenter.NET.Services;

namespace HomeCenter.NET.ViewModels.Commands
{
    public class ProcessViewModel : CommandViewModel
    {
        #region Properties

        public RunnerService.Process Process { get; }
        public Color Color { get; } // TODO: Remove Windows dependency

        #endregion

        #region Constructors

        public ProcessViewModel(RunnerService.Process process) : 
            base(null, process?.Name ?? string.Empty,
                run: true, delete: !process?.IsCompleted ?? false)
        {
            Process = process ?? throw new ArgumentNullException(nameof(process));

            Color = Process.IsCanceled ? Colors.Gold : Process.IsCompleted ? Colors.LightGreen : Colors.Lavender;

        }

        #endregion
    }
}
