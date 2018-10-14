namespace HomeCenter.NET.ViewModels
{
    public class VariableViewModel : CommandBaseViewModel
    {
        #region Constructors

        public VariableViewModel(string name) : 
            base(name, "Please press R to calculate", run: true)
        {
        }

        #endregion
    }
}
