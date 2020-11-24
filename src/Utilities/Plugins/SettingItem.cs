namespace H.NET.Utilities.Plugins
{
    public class SettingItem
    {
        public string Key { get; }
        public object Value { get; }

        public SettingItem(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}