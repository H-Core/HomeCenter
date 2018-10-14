namespace HomeCenter.NET.ViewModels.Commands
{
    public class VariableViewModel : ObjectViewModel
    {
        #region Constructors

        public VariableViewModel(string name) : 
            base(name, "Please press R to calculate", run: true)
        {
        }

        #endregion
    }
}
