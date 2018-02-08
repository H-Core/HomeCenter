using VoiceActions.NET.Runners;
using VoiceActions.NET.Storages;

namespace VoiceActions.NET.Managers
{
    public class RunnerManager : Manager<string>
    {
        #region Properties

        public IRunner Runner { get; set; }

        #endregion

        #region Constructors

        public RunnerManager(IRunner runner = null, IStorage<string> storage = null) : base(storage)
        {
            Runner = runner;
            NewValue += value => Runner?.Run(value);
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Runner?.Dispose();
            Runner = null;

            base.Dispose();
        }

        #endregion
    }
}
