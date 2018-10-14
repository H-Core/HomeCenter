using System;
using H.NET.Storages;

namespace HomeCenter.NET.ViewModels
{
    public class SingleCommandViewModel : CommandBaseViewModel
    {
        #region Properties

        public SingleCommand Command { get; }

        #endregion

        #region Constructors

        public SingleCommandViewModel(SingleCommand command) : base(null, command?.Text, editable: true, run: true, delete: true)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            DescriptionEditAction = d => Command.Text = d;
        }

        #endregion
    }
}
