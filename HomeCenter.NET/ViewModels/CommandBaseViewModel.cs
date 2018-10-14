using System;
using Caliburn.Micro;
using H.NET.Storages;

namespace HomeCenter.NET.ViewModels
{
    public class CommandBaseViewModel : Screen
    {
        #region Properties

        public SingleCommand Command { get; }

        public string CommandName { get; }

        public string DescriptionEditable { get; set; }
        public string DescriptionNotEditable { get; }
        public string Description => IsEditable ? DescriptionEditable : DescriptionNotEditable;

        public bool IsEditable { get; }

        public bool DescriptionEditableIsVisible => IsEditable;
        public bool DescriptionNotEditableIsVisible => !IsEditable;

        public bool CommandNameIsVisible => CommandName != null;
        public bool RunIsVisible { get; }
        public bool EditIsVisible { get; }
        public bool DeleteIsVisible { get; }

        public string HotKey { get; }
        public bool HotKeyIsVisible => HotKey != null;

        #endregion

        #region Constructors

        public CommandBaseViewModel(SingleCommand command, string name, string description, string hotKey = null, bool editable = false,
        bool run = false, bool edit = false, bool delete = false)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));

            CommandName = name;
            DescriptionEditable = description;
            DescriptionNotEditable = description;

            IsEditable = editable;
            RunIsVisible = run;
            EditIsVisible = edit;
            DeleteIsVisible = delete;

            HotKey = hotKey;
        }

        #endregion
    }
}
