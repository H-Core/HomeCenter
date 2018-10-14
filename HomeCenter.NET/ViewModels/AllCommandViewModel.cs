using H.NET.Core;
using H.NET.Core.Utilities;

namespace HomeCenter.NET.ViewModels
{
    public class AllCommandViewModel : CommandBaseViewModel
    {
        #region Properties

        public string Prefix { get; set; }

        #endregion

        #region Constructors

        public AllCommandViewModel((IRunner, string) tuple) : 
            base(null, null, editable: true, run: true)
        {
            var (runner, command) = tuple;
            var values = command.SplitOnlyFirst(' ');

            CommandName = $"{runner.GetType().Name}: {values[0]}";
            Prefix = values[0];
            Description = values[1];
        }

        #endregion
    }
}
