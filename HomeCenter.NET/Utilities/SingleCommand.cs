using System;

namespace HomeCenter.NET.Utilities
{
    public class SingleCommand : TextObject, ICloneable
    {
        #region Constructors

        public SingleCommand()
        {
        }

        public SingleCommand(string text) : base(text)
        {
        }

        #endregion

        #region ICloneable

        public object Clone() => new SingleCommand(Text.Clone() as string);

        #endregion
    }
}
