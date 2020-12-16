namespace HomeCenter.NET.ViewModels.Commands
{
    public class ProcessViewModel : CommandViewModel
    {

        #region Constructors

        public ProcessViewModel() : 
            base(string.Empty, string.Empty,
                run: true, delete: false)
        {
        }

        #endregion
    }
}
