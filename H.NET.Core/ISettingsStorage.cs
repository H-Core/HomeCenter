using System.ComponentModel;
using H.NET.Core.Settings;

namespace H.NET.Core
{
    public interface ISettingsStorage : IStorage<Setting>, INotifyPropertyChanged
    {
        void CopyFrom(string key, Setting setting);
    }
}
