using System;
using System.Windows;
using VoiceActions.NET.Converters;

namespace HomeCenter.NET.Windows
{
    public partial class ConverterWindow
    {
        #region Static methods

        public static (bool isSaved, string token) Show(IConverter converter)
        {
            if (converter is YandexConverter yandexConverter)
            {
                var window = new ConverterWindow(yandexConverter.Key);
                var result = window.ShowDialog() == true;

                return (result, window.Token);
            }

            if (converter is WitAiConverter witAiConverter)
            {
                var window = new ConverterWindow(witAiConverter.Token);
                var result = window.ShowDialog() == true;

                return (result, window.Token);
            }

            return (false, null);
        } 

        public static bool ShowAndSaveIfNeeded(IConverter converter, Action<string> saveAction)
        {
            var (result, newToken) = Show(converter);
            if (!result)
            {
                return false;
            }

            saveAction?.Invoke(newToken);
            return true;
        }

        #endregion

        #region Properties

        public string Token { get; }

        #endregion

        #region Constructors

        public ConverterWindow(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Close(object sender, RoutedEventArgs e) => Close();

        #endregion
    }
}
