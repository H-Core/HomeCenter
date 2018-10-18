using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using HomeCenter.NET.Initializers;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Services;
using HomeCenter.NET.Utilities;
using HomeCenter.NET.ViewModels;
using HomeCenter.NET.ViewModels.Commands;
using HomeCenter.NET.ViewModels.Modules;
using HomeCenter.NET.ViewModels.Settings;
using HomeCenter.NET.Views;

namespace HomeCenter.NET
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer Container { get; set; }
        private MainView MainView { get; set; }

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            Container = new SimpleContainer();

            Container.Instance(Container);

            Container
                .Singleton<IWindowManager, HWindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<HookService>()
                .Singleton<MainService>()
                .Singleton<ModuleService>()
                .Singleton<IpcService>()
                .Singleton<ScreenshotRectangle>()
                .Instance(Settings.Default);

            Container
                .PerRequest<CommandSettingsViewModel>()
                .PerRequest<ModuleSettingsViewModel>()
                .Singleton<CommandsViewModel>()
                .Singleton<SettingsViewModel>()
                .Singleton<PopUpViewModel>()
                .Singleton<MainViewModel>();

            Container
                .PerRequest<StaticModulesInitializer>();

            base.Configure();

            #region Add Visibility Name Convection

            ConventionManager.AddElementConvention<UIElement>(UIElement.VisibilityProperty, "Visibility", "VisibilityChanged");

            void BindVisibilityProperties(IEnumerable<FrameworkElement> frameWorkElements, Type viewModel)
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

            #endregion
        }

        private static void DisposeObject<T>() where T : class, IDisposable
        {
            var obj = IoC.GetInstance(typeof(T), null) as T ?? throw new ArgumentNullException();

            obj.Dispose();
        }

        private static T Get<T>() where T : class 
        {
            return IoC.GetInstance(typeof(T), null) as T ?? throw new ArgumentNullException();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            Initializer.CheckKillAll(e.Args);
            Initializer.CheckNotFirstProcess(e.Args);

            // Catching unhandled exceptions
            WpfSafeActions.Initialize();

            var manager = Get<IWindowManager>();
            var instance = Get<PopUpViewModel>();

            // Create permanent hidden PopupView
            manager.ShowWindow(instance);
            
            var model = Get<MainViewModel>();
            var mainService = Get<MainService>();
            var hookService = Get<HookService>();
            var moduleService = Get<ModuleService>();
            var screenshotRectangle = Get<ScreenshotRectangle>();

            var hWindowManager = manager as HWindowManager ?? throw new ArgumentNullException();

            // Create hidden window(without moment of show/hide)
            MainView = hWindowManager.CreateWindow(model) as MainView ?? throw new ArgumentNullException();

            // TODO: custom window manager is required
            model.IsVisible = e.Args.Contains("/restart") || !Settings.Default.IsStartMinimized;

            Get<StaticModulesInitializer>();
            
            await Initializer.InitializeDynamicModules(mainService, hookService, moduleService, model);

            Initializer.InitializeHooks(mainService, hookService, model, screenshotRectangle);

            Initializer.CheckUpdate(e.Args, mainService);
            Initializer.CheckRun(e.Args, mainService);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            MainView?.Dispose();

            DisposeObject<MainService>();
            DisposeObject<HookService>();
            DisposeObject<ModuleService>();
            DisposeObject<IpcService>();

            Application.Shutdown();
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
#if DEBUG
            MessageBox.Show(e.Exception.ToString(), "An error as occurred", MessageBoxButton.OK, MessageBoxImage.Error);
#else           
            MessageBox.Show(e.Exception.Message, "An error as occurred", MessageBoxButton.OK, MessageBoxImage.Error); 
#endif
        }
    }
}
