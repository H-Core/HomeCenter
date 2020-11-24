using System;

namespace H.NET.Core.Runners
{
    public class RunInformation
    {
        #region Properties

        public Exception Exception { get; set; }
        public bool IsInternal { get; set; }
        public string Description { get; set; }
        public Action<string> Action { get; set; }

        public string RunText { get; set; }

        #endregion

        #region Constructors

        public RunInformation()
        {
        }

        public RunInformation(Exception exception)
        {
            Exception = exception;
            IsInternal = false;
        }

        #endregion

        #region Public methods

        public RunInformation WithException(Exception exception)
        {
            Exception = exception;

            return this;
        }

        #endregion

    }
}
