using System;

namespace HomeCenter.NET.Utilities
{
    public class AppDataFolder : SpecialFolder
    {
        public AppDataFolder(params string[] needToCombinedPathStrings) :
            base(Environment.SpecialFolder.ApplicationData, needToCombinedPathStrings)
        {
        }
    }
}
