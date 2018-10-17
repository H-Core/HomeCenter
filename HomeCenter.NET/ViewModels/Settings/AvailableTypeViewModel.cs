using System;
using H.NET.Core.Extensions;

namespace HomeCenter.NET.ViewModels.Settings
{
    public class AvailableTypeViewModel : ItemViewModel
    {
        #region Properties

        public Type Type { get; }

        #endregion

        #region Constructors

        public AvailableTypeViewModel(Type type) : base(type.Name, type.Assembly.GetName().Name, add: type.AllowMultipleInstance())
        {
            Type = type;
        }

        #endregion
    }
}
