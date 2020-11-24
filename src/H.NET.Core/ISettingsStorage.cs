using System.ComponentModel;
using H.NET.Core.Settings;

namespace H.NET.Core
{
    public interface ISettingsStorage : IStorage<Setting>, INotifyPropertyChanged
    {
        void Set(string key, object value);
    }
}
