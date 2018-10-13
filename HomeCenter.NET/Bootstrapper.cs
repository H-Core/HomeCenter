using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using H.NET.Storages;
using HomeCenter.NET.ViewModels;

namespace HomeCenter.NET
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer Container { get; set; }

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            Container = new SimpleContainer();

            Container.Instance(Container);

            Container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .RegisterInstance(typeof(Command), "command", new Command("test", "test", "B"));

            Container
                .PerRequest<CommandSettingsViewModel>();

            base.Configure();

            ConventionManager.AddElementConvention<UIElement>(UIElement.VisibilityProperty, "Visibility", "VisibilityChanged");

            var baseBindProperties = ViewModelBinder.BindProperties;
            ViewModelBinder.BindProperties =
                (elements, viewModel) =>
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    BindVisibilityProperties(elements, viewModel);

                    // ReSharper disable once PossibleMultipleEnumeration
                    return baseBindProperties(elements, viewModel);
                };

            // Need to override BindActions as well, as it's called first and filters out anything it binds to before
            // BindProperties is called.
            var baseBindActions = ViewModelBinder.BindActions;
            ViewModelBinder.BindActions =
                (elements, viewModel) =>
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    BindVisibilityProperties(elements, viewModel);

                    // ReSharper disable once PossibleMultipleEnumeration
                    return baseBindActions(elements, viewModel);
                };
        }

        private static void BindVisibilityProperties(IEnumerable<FrameworkElement> frameWorkElements, Type viewModel)
        {
            foreach (var frameworkElement in frameWorkElements)
            {
                var propertyName = frameworkElement.Name + "IsVisible";
                var property = viewModel.GetPropertyCaseInsensitive(propertyName);
                if (property == null)
                {
                    continue;
                }

                var convention = ConventionManager
                    .GetElementConvention(typeof(FrameworkElement));
                ConventionManager.SetBindingWithoutBindingOverwrite(
                    viewModel,
                    propertyName,
                    property,
                    frameworkElement,
                    convention,
                    convention.GetBindableProperty(frameworkElement));
            }
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //DisplayRootViewFor<CommandSettingsViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return Container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            Container.BuildUp(instance);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessageBox.Show(e.Exception.Message, "An error as occurred", MessageBoxButton.OK);
        }
    }
}
