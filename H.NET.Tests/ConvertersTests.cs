using System.Threading.Tasks;
using H.NET.Converters;
using H.NET.Tests.Utilities;
using Xunit;

namespace H.NET.Tests
{
    public class ConvertersTests : BaseTests
    {
        [Fact]
        public async Task WitAiConverterTest() => await BaseConverterTest(
            "проверка", ResourcesUtilities.ReadFileAsBytes("speech1.wav"), 
            new WitAiConverter
            {
                Token = "OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS"
            });
    }
}
