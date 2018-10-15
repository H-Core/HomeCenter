using System;
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

        public void Save()
        {
            SaveAction?.Invoke();
            // TODO: alternative solution?
            try
            {
                TryClose(true);
            }
            catch (Exception)
            {
                TryClose();
            }
        }

        public void Cancel()
        {
            CancelAction?.Invoke();
            try
            {
                TryClose(false);
            }
            catch (Exception)
            {
                TryClose();
            }
        }

        #endregion

    }
}
