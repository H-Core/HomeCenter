namespace HomeCenter.NET.ViewModels.Commands
{
    public class VariableViewModel : CommandViewModel
    {
        #region Constructors

        public VariableViewModel(string name) : 
            base(name, "Please press R to calculate", run: true)
        {
        }

        #endregion
    }
}
