using System;
using H.Core.Extensions;

namespace HomeCenter.NET.ViewModels.Settings
{
    public class AvailableTypeViewModel : ItemViewModel
    {
        #region Properties

        public Type Type { get; }

        #endregion

        #region Constructors

        public AvailableTypeViewModel(Type type) : base(type.Name, type.Assembly.GetName().Name ?? string.Empty, add: type.AllowMultipleInstance())
        {
            Type = type;
        }

        #endregion
    }
}
