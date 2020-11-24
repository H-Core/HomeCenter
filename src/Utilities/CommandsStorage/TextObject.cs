namespace H.NET.Storages
{
    public class TextObject
    {
        #region Properties

        public string Text { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public TextObject()
        {
        }

        public TextObject(string text)
        {
            Text = text;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return Text;
        }

        #endregion
    }
}
