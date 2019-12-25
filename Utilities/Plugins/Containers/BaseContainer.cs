using System;

namespace H.NET.Utilities.Plugins.Containers
{
    public class BaseContainer
    {
        #region Properties

        public string FriendlyName { get; }

        #endregion

        #region Constructors

        public BaseContainer(string friendlyName)
        {
            FriendlyName = friendlyName ?? throw new ArgumentNullException(nameof(friendlyName));
        }

        #endregion
    }
}
