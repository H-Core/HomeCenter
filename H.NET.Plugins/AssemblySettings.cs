using System;

namespace H.NET.Plugins
{
    public class AssemblySettings
    {
        #region Properties

        public string Name { get; set; }
        public string SubPath { get; set; }
        public bool IsUsed { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string OriginalPath { get; set; }

        #endregion

    }
}