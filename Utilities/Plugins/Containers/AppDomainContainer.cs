using System;
using System.IO;
using H.NET.Utilities.Plugins.Extensions;

namespace H.NET.Utilities.Plugins.Containers
{
    public class AppDomainContainer : BaseContainer, IContainer
    {
        public AppDomain AppDomain { get; }

        public AppDomainContainer(string friendlyName) : base(friendlyName)
        {
            AppDomain = AppDomain.CreateDomain(FriendlyName);
        }

        public void Load(Stream assemblyStream)
        {
            AppDomain.Load(assemblyStream.ToArray());
        }
    }
}
