using System;
using H.NET.Storages;
using H.Utilities;

namespace HomeCenter.NET.ViewModels.Commands
{
    public class UserCommandViewModel : CommandViewModel
    {
        #region Properties

        public Command Command { get; }

        #endregion

        #region Constructors

        public UserCommandViewModel(Command? command) : 
            base(command?.FirstKeyText ?? string.Empty,
                command?.FirstDataText ?? string.Empty,
                command?.HotKey,
                run: true, edit: true, delete: true)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        #endregion
    }
}
