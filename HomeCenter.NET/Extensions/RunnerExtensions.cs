using System;
using System.IO;
using H.NET.Core.Runners;

namespace HomeCenter.NET.Extensions
{
    public static class RunnerExtensions
    {
        #region CheckPathAndRun

        public static void CheckPathAndRun<T>(this Runner runner, string path, Func<string, T> action)
        {
            runner.CheckPathAndRun(path, o => { action?.Invoke(o); });
        }

        public static void CheckPathAndRun(this Runner runner, string path, Action<string> action)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                runner.Print(@"Path is empty"); //TODO: may be need to use Module.Log?
                return;
            }
            if (!File.Exists(path))
            {
                runner.Print(@"Path is not exists");
                return;
            }

            action?.Invoke(path);
        }

        #endregion
    }
}
