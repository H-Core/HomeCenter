using System;

namespace HomeCenter.NET.Utilities
{
    public class AppDataFile : SpecialFolderFile
    {
        public AppDataFile(params string[] needToCombinedPathStrings) : 
            base(Environment.SpecialFolder.ApplicationData, needToCombinedPathStrings)
        {
        }
    }
}
