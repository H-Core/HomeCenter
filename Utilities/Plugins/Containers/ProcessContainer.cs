using System.Diagnostics;

namespace H.NET.Utilities.Plugins.Containers
{
    public class ProcessContainer : BaseContainer, IContainer
    {
        public Process Process { get; }

        public ProcessContainer(string friendlyName) : base(friendlyName)
        {
        }
    }
}
