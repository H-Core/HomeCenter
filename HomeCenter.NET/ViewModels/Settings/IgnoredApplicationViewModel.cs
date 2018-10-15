namespace HomeCenter.NET.ViewModels.Settings
{
    public class IgnoredApplicationViewModel : ItemViewModel
    {
        #region Constructors

        public IgnoredApplicationViewModel(string path) : base(path, delete: true)
        {
        }

        #endregion
    }
}
