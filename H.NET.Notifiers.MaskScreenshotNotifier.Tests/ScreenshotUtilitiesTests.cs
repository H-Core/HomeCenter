using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using H.NET.Notifiers.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.NET.Notifiers.MaskScreenshotNotifier.Tests
{
    [TestClass]
    public class ScreenshotUtilitiesTests
    {
        private static string GetFullPath(string name) =>
            $"C:\\Users\\haven\\Source\\Repos\\UpworkNotifier\\UpworkNotifier\\Properties\\Images\\{name}";

        private static void BaseTest(bool expected, string name1, string name2)
        {
            var mat1 = new Mat(GetFullPath(name1), ImreadModes.Grayscale);
            var mat2 = new Mat(GetFullPath(name2), ImreadModes.Grayscale);
            var difference = ScreenshotUtilities.GetDifference(mat1, mat2, mat2);

            Console.WriteLine($@"Difference: {difference}. Name1: {name1}, Name2: {name2}");
            Assert.AreEqual(expected, ScreenshotUtilities.IsEquals(difference));
        }

        [TestMethod]
        public void GetDifference_True()
        {
            BaseTest(true, "upwork_message_1920.bmp", "upwork_message_1920.bmp");
            BaseTest(true, "upwork_true_1_1920.bmp", "upwork_message_1920.bmp");
        }

        [TestMethod]
        public void GetDifference_False()
        {
            BaseTest(false, "upwork_false_1_1920.bmp", "upwork_message_1920.bmp");
        }
    }
}
