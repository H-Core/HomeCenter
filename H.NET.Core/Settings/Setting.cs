using System;

namespace H.NET.Core.Settings
{
    public class Setting : BaseSetting
    {
        public Type Type => DefaultValue.GetType();

        public Func<object, bool> CheckFunc { get; set; }
        public bool IsValid() => CheckFunc?.Invoke(Value) ?? true;

        public Action<object> SetAction { get; set; }
        public void Set() => SetAction?.Invoke(Value);
    }
}
