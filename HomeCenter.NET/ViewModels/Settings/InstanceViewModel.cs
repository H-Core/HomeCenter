using System;
using Caliburn.Micro;
using H.NET.Core;
using H.NET.Core.Extensions;
using H.NET.Plugins;

namespace HomeCenter.NET.ViewModels.Settings
{
    public class InstanceViewModel : Screen
    {
        #region Properties

        private string _name;
        public string Name {
            get => _name;
            set {
                _name = value;
                NotifyOfPropertyChange(nameof(Name));
            }
        }

        private string _description;
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

        public RuntimeObject<IModule> Instance { get; }

        #endregion

        #region Constructors

        public InstanceViewModel(string name, RuntimeObject<IModule> instance)
        {
            Instance = instance ?? throw new ArgumentNullException(nameof(Module));
            
            var module = instance.Value;
            var deletingAllowed = instance.Exception != null || (instance.Type?.AllowMultipleInstance() ?? false);

            Name = name;
            Description = module?.Name ?? instance.Exception?.Message ?? string.Empty;

            IsValid = module != null;
            IsEnabled = instance.IsEnabled;

            EditIsVisible = module != null && module.Settings?.Count > 0;
            EnableIsVisible = !instance.IsStatic && instance.Exception == null;
            RenameIsVisible = !instance.IsStatic && (instance.Type?.AllowMultipleInstance() ?? false);
            DeleteIsVisible = !instance.IsStatic && deletingAllowed;
        }

        #endregion

    }
}
