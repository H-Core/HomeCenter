namespace HomeCenter.NET.ViewModels.Settings
{
    public class AssemblyViewModel : ItemViewModel
    {
        #region Constructors

        public AssemblyViewModel(string name, bool update) : base(name, delete: true, update: update)
        {
        }

        #endregion
    }
}
