using System;
using H.NET.Storages;

namespace HomeCenter.NET.ViewModels.Commands
{
    public class UserCommandViewModel : ObjectViewModel
    {
        #region Properties

        public Command Command { get; }

        #endregion

        #region Constructors

        public UserCommandViewModel(Command command) : 
            base(command?.FirstKeyText,
                command?.FirstDataText,
                command?.HotKey,
                run: true, edit: true, delete: true)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        #endregion
    }
}
