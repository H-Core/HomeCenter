using System;

namespace H.NET.Utilities
{
    public class AppDataFolder : SpecialFolder
    {
        public AppDataFolder(params string[] needToCombinedPathStrings) :
            base(Environment.SpecialFolder.ApplicationData, needToCombinedPathStrings)
        {
        }
    }
}
