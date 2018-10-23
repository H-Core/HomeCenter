namespace HomeCenter.NET.ViewModels.Utilities
{
    public class RenameViewModel : SaveCancelViewModel
    {
        #region Properties

        public string OldName { get; set; }
        public string NewName { get; set; }

        #endregion

        #region Constructors

        public RenameViewModel(string oldName, string newName = null)
        {
            OldName = oldName;
            NewName = newName;
        }

        #endregion
    }
}
