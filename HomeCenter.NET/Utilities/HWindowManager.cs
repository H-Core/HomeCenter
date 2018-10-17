using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;

namespace HomeCenter.NET.Utilities
{
    public class HWindowManager : WindowManager
    {
        public Window CreateWindow(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            return CreateWindow(rootModel, false, context, settings);
        }
    }
}
