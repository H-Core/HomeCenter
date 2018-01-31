using System;
using VoiceActions.NET.Utilities;
using Xunit;

namespace VoiceActions.NET.Tests
{
    public class StringExtensionsTests
    {
        private void BaseTwoStringTupleTest((string, string) expected, (string, string) actual)
        {
            Assert.Equal(expected.Item1, actual.Item1);
            Assert.Equal(expected.Item2, actual.Item2);
        }

        [Fact]
        public void SplitOnlyFirstTest()
        {
            BaseTwoStringTupleTest(("test", "test test"), "test test test".SplitOnlyFirst(' '));
            BaseTwoStringTupleTest(("test", "test"), "test test".SplitOnlyFirst(' '));
            BaseTwoStringTupleTest(("test", ""), "test ".SplitOnlyFirst(' '));
            BaseTwoStringTupleTest(("test", null), "test".SplitOnlyFirst(' '));
            BaseTwoStringTupleTest(("", ""), " ".SplitOnlyFirst(' '));
            BaseTwoStringTupleTest(("", null), "".SplitOnlyFirst(' '));

            Assert.Throws<ArgumentNullException>(() =>
            {
                string test = null;
                test.SplitOnlyFirst(' ');
            });
        }
    }
}
