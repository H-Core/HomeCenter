using System.IO;
using NUnit.Framework;

namespace VoiceActions.NET.Tests.Utilities
{
    public static class TestUtilities
    {
        public static string OutputPath => TestContext.CurrentContext.TestDirectory;
        public static string ProjectPath => Path.GetDirectoryName(Path.GetDirectoryName(OutputPath));
        public static string SolutionPath => Path.GetDirectoryName(ProjectPath);
        public static string TestFilesPath => Path.Combine(ProjectPath, "TestFiles");
        public static string RawSpeechPath => Path.Combine(TestFilesPath, "RawSpeech");

        public static string GetFullPathForRawSpeech(string name) => Path.Combine(RawSpeechPath, name);
        public static byte[] GetRawSpeech(string name) => File.ReadAllBytes(GetFullPathForRawSpeech(name));
    }
}
