namespace H.NET.Core.Utilities
{
    public class TextDeferredEventArgs : DeferredEventArgs
    {
        public static TextDeferredEventArgs Create(string text) => new TextDeferredEventArgs(text);

        public string Text { get; }

        public TextDeferredEventArgs(string text)
        {
            Text = text;
        }
    }
}
