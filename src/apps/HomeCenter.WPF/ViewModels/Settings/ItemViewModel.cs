using Caliburn.Micro;

namespace HomeCenter.NET.ViewModels.Settings
{
    public class ItemViewModel : Screen
    {
        #region Properties

        public string Name { get; set; }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyOfPropertyChange(nameof(Description));
            }
        }

        public bool ItemNameIsVisible => Name != null;
        public bool UpdateIsVisible { get; protected set; }
        public bool AddIsVisible { get; protected set; }
        public bool EditIsVisible { get; protected set; }
        public bool DeleteIsVisible { get; protected set; }

        #endregion

        #region Constructors

        protected ItemViewModel(string? name, string description,
            bool update = false, bool edit = false, bool add = false, bool delete = false)
        {
            Name = name ?? string.Empty;
            Description = description;

            UpdateIsVisible = update;
            EditIsVisible = edit;
            AddIsVisible = add;
            DeleteIsVisible = delete;
        }

        protected ItemViewModel(string description,
            bool update = false, bool edit = false, bool add = false, bool delete = false) :
            this(null, description, update, edit, add, delete)
        {
        }

        #endregion
    }
}
