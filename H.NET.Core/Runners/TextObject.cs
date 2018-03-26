namespace H.NET.Core.Runners
{
    public class TextObject
    {
        #region Properties

        public string Text { get; set; }

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
