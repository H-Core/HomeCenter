using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using H.NET.Core.Managers;
using HomeCenter.NET.Initializers;
using HomeCenter.NET.Input;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Services;
using HomeCenter.NET.Utilities;
using HomeCenter.NET.Utilities.HookModules;
using HomeCenter.NET.ViewModels;
using HomeCenter.NET.ViewModels.Commands;
using HomeCenter.NET.ViewModels.Modules;
using HomeCenter.NET.ViewModels.Settings;
using HomeCenter.NET.ViewModels.Utilities;
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

            //Container.Instance(Container);

            Container
                .Singleton<IWindowManager, HWindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<HookService>()
                .Singleton<ModuleService>()
                .Singleton<RunnerService>()
                .Singleton<IpcService>()
                .Singleton<StorageService>()
                .Singleton<BaseManager>()
                .Singleton<ScreenshotModule>()
                .Instance(Settings.Default);

            Container
                .PerRequest<CommandSettingsViewModel>()
                .PerRequest<ModuleSettingsViewModel>()
                .PerRequest<CommandsViewModel>()
                .Singleton<SettingsViewModel>()
                .Singleton<PopupViewModel>()
                .Singleton<MainViewModel>();

            Container
                .PerRequest<StaticModulesInitializer>()
                .PerRequest<HookInitializer>();

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

            #region Key Triggers

            var defaultCreateTrigger = Parser.CreateTrigger;

            Parser.CreateTrigger = (target, triggerText) =>
            {
                if (triggerText == null)
                {
                    return defaultCreateTrigger(target, null);
                }

                var triggerDetail = triggerText
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                var splits = triggerDetail.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                switch (splits[0])
                {
                    case "KeyUp":
                        return KeyUpTrigger.FromText(splits[1]);
                }

                return defaultCreateTrigger(target, triggerText);
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
            InitializeHelper.CheckKillAll(e.Args);
            InitializeHelper.CheckNotFirstProcess(e.Args);

            // Catching unhandled exceptions
            WpfSafeActions.Initialize();

            var manager = Get<IWindowManager>();
            var instance = Get<PopupViewModel>();

            // Create permanent hidden PopupView
            manager.ShowWindow(instance);
            
            var model = Get<MainViewModel>();
            var runnerService = Get<RunnerService>();
            var hookService = Get<HookService>();
            var moduleService = Get<ModuleService>();

            var hWindowManager = manager as HWindowManager ?? throw new ArgumentNullException();

            // Create hidden window(without moment of show/hide)
            MainView = hWindowManager.CreateWindow(model) as MainView ?? throw new ArgumentNullException();

            // TODO: custom window manager is required
            model.IsVisible = e.Args.Contains("/restart") || !Settings.Default.IsStartMinimized;

            Get<StaticModulesInitializer>();
            
            await InitializeHelper.InitializeDynamicModules(runnerService, hookService, moduleService, model);

            Get<HookInitializer>();

            InitializeHelper.CheckUpdate(e.Args, runnerService);
            InitializeHelper.CheckRun(e.Args, runnerService);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            MainView?.Dispose();

            DisposeObject<BaseManager>();
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
