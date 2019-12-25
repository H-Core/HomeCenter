using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Yandex.Cloud.Ai.Stt.V2;

namespace YandexConverter
{
    public class YandexConverter : Options
    {
        public class VoiceStreamArgs : EventArgs
        {
            public byte[] Buffer { get; set; }
            public int BytesRecorded { get; set; }

            public VoiceStreamArgs(byte[] buffer, int bytesRecorded)
            {
                Buffer = buffer;
                BytesRecorded = bytesRecorded;
            }
        }

        private RecognitionSpec StreamingSpecification { get; set; }
        private RecognitionConfig StreamingConfig { get; set; }
        private StreamingRecognitionRequest StreamingRequest { get; set; }

        private AsyncDuplexStreamingCall<StreamingRecognitionRequest, StreamingRecognitionResponse> StreamingCall
        {
            get;
            set;
        }

        private Task<Task<List<string>>> WorkResult { get; set; }
        private Metadata MetadataObject { get; set; }
        private Recorder RecorderObject { get; set; }

        private bool IsWriteActive { get; set; }

        public static event EventHandler<VoiceStreamArgs> OnDataAvailable;

        #region Constructor

        public YandexConverter(Recorder rec)
        {
            //ToDo: todo over anonym classes?
            StreamingSpecification = new RecognitionSpec
            {
                LanguageCode = "ru-RU",
                ProfanityFilter = false,
                Model = "general",
                PartialResults = true,
                AudioEncoding = RecognitionSpec.Types.AudioEncoding.Linear16Pcm,
                SampleRateHertz = 8000
            };
            StreamingConfig = new RecognitionConfig
            {
                Specification = StreamingSpecification,
                FolderId = FOLDER_ID
            };
            StreamingRequest = new StreamingRecognitionRequest()
            {
                Config = StreamingConfig
            };
            MetadataObject = new Metadata
            {
                {"authorization", $"Bearer {IAM_TOKEN}"}
            };
            RecorderObject = rec;
        }

        private AsyncDuplexStreamingCall<StreamingRecognitionRequest, StreamingRecognitionResponse>
            CreateStreamingCall()
        {
            var channelCredentials = new SslCredentials();
            var channel = new Channel("stt.api.cloud.yandex.net", 443, channelCredentials);
            var client = new SttService.SttServiceClient(channel);
            return client.StreamingRecognize(MetadataObject);
        }

        #endregion

        public async Task InitializationTask()
        {
            StreamingCall = this.CreateStreamingCall();

            await StreamingCall.RequestStream.WriteAsync(StreamingRequest);

            IsWriteActive = true;

            OnDataAvailable += (sender, args) =>
            {
                IsWriteActive = true;
                StreamingCall.RequestStream.WriteAsync(
                    new StreamingRecognitionRequest()
                    {
                        AudioContent = ByteString.CopyFrom(args.Buffer, 0, args.BytesRecorded)
                    }).Wait();
                IsWriteActive = false;

            };
        }

        public void Start()
        {
            WorkResult = Task.Run(async () =>
            {
                var testList = new List<string>();

                while (await StreamingCall.ResponseStream.MoveNext())
                {
                    var note = StreamingCall.ResponseStream.Current;
                    testList.Add($"{note}");
                    Debug.WriteLine($"Received {DateTime.Now.ToLongTimeString()}:  {note}");
                }

                return Task.FromResult(testList);
            });
            RecorderObject.Start();
        }

        public async Task<List<string>> StopAndGetResponseTask()
        {
            RecorderObject.Stop();
            try
            {
                while (IsWriteActive)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(25));
                }

                await StreamingCall.RequestStream.CompleteAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"VoiceStop EXCEPTION : {e}");
            }

            return await WorkResult.Result;
        }

        public void ProcessData(byte[] data)
        {
            OnDataAvailable?.Invoke(this, new VoiceStreamArgs(data, data.Length));
        }
    }
}
