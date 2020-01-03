#if NETFRAMEWORK
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.NET.Converters.IntegrationTests
{

    [TestClass]
    public class SystemSpeechConverterTests
    {
        [TestMethod]
        public async Task ConvertTest_RealTime()
        {
            using var converter = new SystemSpeechConverter();

            await converter.ConvertAsync(Array.Empty<byte>());

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}
#endif
