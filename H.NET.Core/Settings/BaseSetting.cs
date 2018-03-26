namespace H.NET.Core.Settings
{
    public class BaseSetting
    {
        #region Properties

        public string Key { get; set; }
        public object Value { get; set; }
        public object DefaultValue { get; set; }
        public SettingType SettingType { get; set; }

        #endregion

        #region Public methods

        public void CopyFrom(BaseSetting setting)
        {
            Key = setting.Key;
            Value = setting.Value;
            DefaultValue = setting.DefaultValue;
            SettingType = setting.SettingType;
        }

        public BaseSetting Copy()
        {
            var setting = new BaseSetting();
            setting.CopyFrom(this);

            return setting;
        }

        #endregion

    }
}
