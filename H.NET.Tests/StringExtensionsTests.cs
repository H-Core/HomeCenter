using System;
using H.NET.Core.Utilities;
using Xunit;

namespace H.NET.Tests
{
    public class StringExtensionsTests
    {
        private static void BaseTwoStringTupleTest((string, string) expected, string[] actual)
        {
            Assert.Equal(expected.Item1, actual[0]);
            Assert.Equal(expected.Item2, actual[1]);
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
