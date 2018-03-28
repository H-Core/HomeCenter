using System.Threading.Tasks;
using H.NET.Synthesizers;
using Xunit;

namespace H.NET.Tests
{
    public class SynthesizerTests : BaseTests
    {
        [Fact]
        public async Task YandexSynthesizerTest() => await BaseSynthesizerTest(
            "проверка", new YandexSynthesizer
            {
                Key = "1ce29818-0d15-4080-b6a1-ea5267c9fefd",
                Lang = "ru-RU"
            });
    }
}
