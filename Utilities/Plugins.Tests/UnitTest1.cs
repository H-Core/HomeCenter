using System;
using System.Runtime.Loader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plugins.Tests.Utilities;

namespace Plugins.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var assemblyStream = ResourcesUtilities.ReadFileAsStream("H.NET.Core.dll");
            var container = new AssemblyLoadContext("Modules", true);

            container.LoadFromStream(assemblyStream);

            Console.WriteLine();
        }
    }
}
