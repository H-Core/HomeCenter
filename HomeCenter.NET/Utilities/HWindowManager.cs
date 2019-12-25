using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace HomeCenter.NET.Utilities
{
    public class HWindowManager : WindowManager
    {
        public async Task<Window> CreateWindowAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            return await CreateWindowAsync(rootModel, false, context, settings);
        }
    }
}
