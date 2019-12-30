using System;
using H.NET.Storages;

namespace HomeCenter.NET.ViewModels.Commands
{
    public class SingleCommandViewModel : CommandViewModel
    {
        #region Properties

        public SingleCommand Command { get; }

        #endregion

        #region Constructors

        public SingleCommandViewModel(SingleCommand command) : base(string.Empty, command?.Text ?? string.Empty, editable: true, run: true, delete: true)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            DescriptionEditAction = d => Command.Text = d;
        }

        #endregion
    }
}
