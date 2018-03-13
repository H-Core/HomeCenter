using System;

namespace H.Storages.Utilities
{
    public class AppDataFile : SpecialFolderFile
    {
        public AppDataFile(params string[] needToCombinedPathStrings) : 
            base(Environment.SpecialFolder.ApplicationData, needToCombinedPathStrings)
        {
        }
    }
}
