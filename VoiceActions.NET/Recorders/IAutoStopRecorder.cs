namespace VoiceActions.NET.Recorders
{
    public interface IAutoStopRecorder : IRecorder
    {
        bool AutoStopEnabled { get; set; }
    }
}
