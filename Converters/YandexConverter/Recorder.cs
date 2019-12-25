using H.NET.Recorders;
using NAudio.Wave;

namespace YandexConverter
{
    public class Recorder
    {
        public NAudioRecorder NRecorder { get; set; }
        public BufferedWaveProvider Provider { get; set; }
        public WaveOutEvent Output { get; set; }
        public YandexConverter MainYandexConverter { get; set; }

        public Recorder()
        {
            NRecorder = new NAudioRecorder();
            Provider = new BufferedWaveProvider(NRecorder.WaveIn.WaveFormat);
            Output = new WaveOutEvent();
            Output.Init(Provider);
            Output.Play();
            NRecorder.NewData += (sender, args) => Provider.AddSamples(args.Buffer, 0, args.BytesRecorded);
            NRecorder.NewData += (sender, args) => ProcessVoiceStream(args.Buffer);
            MainYandexConverter = new YandexConverter(this);
        }

        public void Start()
        {
            NRecorder.Start();
        }

        public void Stop()
        {
            NRecorder.Stop();
        }

        public void ProcessVoiceStream(byte[] buffer)
        {
            MainYandexConverter.ProcessData(buffer);
        }

    }
}