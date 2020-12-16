using Caliburn.Micro;

namespace HomeCenter.NET.ViewModels.Utilities
{
    public class MessageBoxViewModel : Screen
    {
        #region Properties

        public string Text { get; }
        public string Title { get; }

        #endregion

        #region Constructors

        public MessageBoxViewModel(string text, string? title = null)
        {
            Text = text ?? string.Empty;
            Title = title ?? string.Empty;
        }

        #endregion
    }
}
