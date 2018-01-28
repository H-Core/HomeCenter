namespace HomeCenter.NET.Utilities
{
    public class ConsoleEventArgs
    {
        public string Text { get; set; }

        public ConsoleEventArgs(string text)
        {
            Text = text;
        }
    }
}
