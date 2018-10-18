using System.Reflection;

namespace HomeCenter.NET.Utilities
{
    public static class Options
    {
        public static string FilePath => Assembly.GetExecutingAssembly().Location; //Application.ResourceAssembly.Location
        public const string ApplicationName = "HomeCenter.NET";
        public const string CompanyName = "HomeCenter.NET";

    }
}
