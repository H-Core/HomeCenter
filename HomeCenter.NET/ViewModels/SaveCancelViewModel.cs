using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Action = System.Action;

namespace HomeCenter.NET.ViewModels
{
    public class SaveCancelViewModel : Screen
    {
        #region Properties

        protected Action SaveAction { get; set; }
        protected Action CancelAction { get; set; }

        #endregion

        #region Constructors

        public SaveCancelViewModel(Action saveAction = null, Action cancelAction = null)
        {
            SaveAction = saveAction;
            CancelAction = cancelAction;
        }

        #endregion

        #region Public methods

        public async Task SaveAsync()
        {
            SaveAction?.Invoke();
            // TODO: alternative solution?
            try
            {
                await TryCloseAsync(true);
            }
            catch (Exception)
            {
                await TryCloseAsync();
            }
        }

        public async Task CancelAsync()
        {
            CancelAction?.Invoke();
            try
            {
                await TryCloseAsync(false);
            }
            catch (Exception)
            {
                await TryCloseAsync();
            }
        }

        #endregion

    }
}
