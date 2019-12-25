using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace YandexConverter.IntegrationTests
{
    [TestClass]
    public class YandexVoiceRecognitionTests
    {
        [TestMethod]
        public async Task Test()
        {
            YandexTest("Здравствуй мир", await YandexVoiceRecongnition(5));
        }

        public async Task<string> YandexVoiceRecongnition(int speechTime)
        {
            var basis = new Recorder();
            await basis.MainYandexConverter.InitializationTask();
            basis.MainYandexConverter.Start();

            Debug.WriteLine($"TaskDelay Started at:  {DateTime.Now.ToLongTimeString()}");
            await Task.Delay(TimeSpan.FromSeconds(speechTime));

            var responseList = ToGrpcAnswerList(await basis.MainYandexConverter.StopAndGetResponseTask());

            var finalAnswer = GetFinalGrpcAnswer(responseList);

            return finalAnswer.Chunks.FirstOrDefault()?.Alternatives.FirstOrDefault()?.Text;
        }

        
        public void YandexTest(string expected, string actual)
        {
            Assert.AreEqual(expected, actual);
        }

        

        public GrpcAnswer GetFinalGrpcAnswer(List<GrpcAnswer> list)
        {
            return list.Where(l => l.Chunks.First().Final).ToList().LastOrDefault();
        }
        public List<GrpcAnswer> ToGrpcAnswerList(List<string> responseList)
        {
            var grpcList = new List<GrpcAnswer>();
            foreach (var responseString in responseList)
            {
                grpcList.Add(JsonConvert.DeserializeObject<GrpcAnswer>(responseString));
            }

            return grpcList;
        }
    }
}
