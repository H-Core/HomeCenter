using System;

namespace HomeCenter.NET.Utilities
{
    public class SingleKey : TextObject, ICloneable
    {
        #region Constructors

        public SingleKey()
        {
        }

        public SingleKey(string text) : base(text)
        {
        }

        #endregion

        #region ICloneable

        public object Clone() => new SingleKey(Text.Clone() as string);

        #endregion
    }
}
