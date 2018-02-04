using System.Reflection;

namespace HomeCenter.NET.Utilities
{
    public static class Options
    {
        public static string FileName => Assembly.GetExecutingAssembly().Location;
    }
}
