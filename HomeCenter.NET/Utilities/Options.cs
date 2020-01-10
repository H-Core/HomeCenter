using System.Reflection;

namespace HomeCenter.NET.Utilities
{
    public static class Options
    {
        public static string FilePath => Assembly.GetExecutingAssembly().Location.Replace(".dll", "") + ".exe"; //Application.ResourceAssembly.Location
        public const string ApplicationName = "HomeCenter.NET";
        public const string CompanyName = "HomeCenter.NET";

    }
}
