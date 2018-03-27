using H.NET.Core.Recorders;

namespace H.NET.Core.Managers
{
    public class BaseManager : ParentRecorder
    {
        #region Properties

        public new IRecorder Recorder {
            get => base.Recorder;
            set {
                if (value == null && base.Recorder != null)
                {
                    base.Recorder.Stopped -= OnStoppedRecorder;
                }

                base.Recorder = value;

                if (value != null)
                {
                    base.Recorder.Stopped += OnStoppedRecorder;
                }
            }
        }

        public IConverter Converter { get; set; }

        public string Text { get; private set; }

        #endregion

        #region Events

        public delegate void TextDelegate(string key);
        public event TextDelegate NewText;
        private void OnNewText() => NewText?.Invoke(Text);
        
        protected override VoiceActionsEventArgs CreateArgs() => new VoiceActionsEventArgs
        {
            Recorder = Recorder,
            Converter = Converter,
            Data = Data,
            Text = Text
        };

        #endregion

        #region Constructors

        #endregion

        #region Public methods

        public void ProcessText(string text)
        {
            Text = text;
            OnNewText();
        }

        public async void ProcessSpeech(byte[] bytes)
        {
            if (Converter == null)
            {
                Log("Converter is not found");
                return;
            }

            ProcessText(await Converter.Convert(bytes));
        }

        public override void Start()
        {
            Text = null;
            base.Start();
        }

        public override void Stop()
        {
            if (Recorder == null)
            {
                Log("Recorder is not found");
                return;
            }

            Recorder.Stop();
        }

        public void Change()
        {
            if (!IsStarted)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        #endregion

        #region Event handlers

        private void OnStoppedRecorder(object sender, VoiceActionsEventArgs args)
        {
            IsStarted = false;

            if (Recorder == null)
            {
                Log("Recorder is not found");
                return;
            }

            Data = Recorder.Data;
            OnStopped(CreateArgs());

            ProcessSpeech(Data);
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Recorder?.Dispose();
            Recorder = null;

            Converter?.Dispose();
            Converter = null;

            base.Dispose();
        }

        #endregion
    }
}
