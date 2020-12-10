using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using H.Core;
using H.Core.Extensions;
using HomeCenter.NET.Extensions;

namespace HomeCenter.NET.ViewModels.Settings
{
    public class InstanceViewModel : Screen
    {
        #region Properties

        private string _name = string.Empty;
        public string Name {
            get => _name;
            set {
                _name = value;
                NotifyOfPropertyChange(nameof(Name));
            }
        }

        private string _description = string.Empty;
        public string Description {
            get => _description;
            set {
                _description = value;
                NotifyOfPropertyChange(nameof(Description));
            }
        }

        private bool _isEnabled;
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                NotifyOfPropertyChange(nameof(IsEnabled));
            }
        }

        private bool _isValid = true;
        public bool IsValid {
            get => _isValid;
            set {
                _isValid = value;
                NotifyOfPropertyChange(nameof(IsValid));
            }
        }

        public bool InstanceNameIsVisible => Name != null;
        public bool EnableIsVisible { get; protected set; }
        public bool RenameIsVisible { get; protected set; }
        public bool EditIsVisible { get; protected set; }
        public bool DeleteIsVisible { get; protected set; }

        //public RuntimeObject<IModule> Instance { get; }

        #endregion

        #region Constructors

        public InstanceViewModel(string name)
        {
            //Instance = instance ?? throw new ArgumentNullException(nameof(Module));
            
            //var module = Instance.Value;
            //var deletingAllowed = Instance.Exception != null || (Instance.Type?.AllowMultipleInstance() ?? false);

            Name = name?? string.Empty;
            //Description = module?.Name ?? Instance.Exception?.Message ?? string.Empty;

            //IsValid = module != null;
            //IsEnabled = Instance.IsEnabled;

            //EditIsVisible = module != null && module.Settings?.Count > 0;
            //EnableIsVisible = !Instance.IsStatic && Instance.Exception == null;
            //RenameIsVisible = !Instance.IsStatic && (Instance.Type?.AllowMultipleInstance() ?? false);
            //DeleteIsVisible = !Instance.IsStatic && deletingAllowed;
        }

        #endregion

        #region Public methods

        public async Task ShowDescriptionAsync()
        {
            await this.ShowMessageBoxAsync(
            //    Instance.Value?.Name ?? Instance.Exception?.ToString() ?? string.Empty,
            string.Empty, "Description");
        }

        #endregion
    }
}
