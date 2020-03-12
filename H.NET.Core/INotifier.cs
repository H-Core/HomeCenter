using System;

namespace H.NET.Core
{
    public interface INotifier : IModule
    {
        event EventHandler EventOccurred;
    }
}
