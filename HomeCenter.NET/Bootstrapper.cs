using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using H.NET.Core;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Runners;
using HomeCenter.NET.Services;
using HomeCenter.NET.Utilities;
using HomeCenter.NET.ViewModels;
using HomeCenter.NET.ViewModels.Commands;
using HomeCenter.NET.ViewModels.Modules;
using HomeCenter.NET.ViewModels.Settings;

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
                .Singleton<HookService>()
                .Singleton<MainService>()
                .Instance(Settings.Default);

            Container
                .PerRequest<CommandSettingsViewModel>()
                .PerRequest<ModuleSettingsViewModel>()
                .Singleton<CommandsViewModel>()
                .Singleton<SettingsViewModel>()
                .Singleton<PopUpViewModel>()
                .Singleton<MainViewModel>();

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

        private static void Run(string command)
        {
            var service = IoC.GetInstance(typeof(MainService), null) as MainService ?? throw new ArgumentNullException();

            service.Run(command);
        }

        private static void DisposeObject<T>() where T : class, IDisposable
        {
            var obj = IoC.GetInstance(typeof(T), null) as T ?? throw new ArgumentNullException();

            obj.Dispose();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            DisposeObject<MainViewModel>();
            DisposeObject<MainService>();
            DisposeObject<HookService>();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            var manager = GetInstance(typeof(IWindowManager), null) as IWindowManager ?? throw new ArgumentNullException();
            var instance = GetInstance(typeof(PopUpViewModel), null) as PopUpViewModel ?? throw new Exception(@"PopUpViewModel Instance is null");

            // Create permanent hidden PopupView
            manager.ShowWindow(instance);

            //DisplayRootViewFor<MainViewModel>();

            var model = IoC.GetInstance(typeof(MainViewModel), null) as MainViewModel ?? throw new ArgumentNullException();
            var mainService = IoC.GetInstance(typeof(MainService), null) as MainService ?? throw new ArgumentNullException();

            //manager?.ShowWindow(IoC.GetInstance(typeof(MainViewModel), null));


            var isKillAll = e.Args.Contains("/killall");
            if (isKillAll)
            {
                Process.GetProcessesByName(Options.ApplicationName)
                    .Where(i => i.Id != Process.GetCurrentProcess().Id)
                    .AsParallel()
                    .ForAll(i => i.Kill());
            }

            var isRestart = e.Args.Contains("/restart");

            // If current process is not first
            if (Process.GetProcessesByName(Options.ApplicationName).Length > 1 &&
                !isRestart && !isKillAll)
            {
                Application.Shutdown();
                return;
            }

            // Catching unhandled exceptions
            WpfSafeActions.Initialize();

            manager.ShowWindow(model);
            // TODO: custom window manager is required
            model.IsVisible = isRestart || !Settings.Default.IsStartMinimized;


            var isUpdating = e.Args.Contains("/updating");

            #region Static Runners

            var staticRunners = new List<IRunner>
            {
                new DefaultRunner(model.Print, model.Say, model.Search),
                new KeyboardRunner(),
                new WindowsRunner(),
                new ClipboardRunner
                {
                    ClipboardAction = command => Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(command)),
                    ClipboardFunc = () => Application.Current.Dispatcher.Invoke(Clipboard.GetText)
                },
                new UiRunner
                {
                    // TODO: refactor
                    RestartAction = command => Application.Current.Dispatcher.Invoke(() => mainService.Restart(command)),
                    UpdateRestartAction = command => Application.Current.Dispatcher.Invoke(() => mainService.RestartWithUpdate(command)),
                    ShowUiAction = () => Application.Current.Dispatcher.Invoke(() => model.IsVisible = !model.IsVisible),
                    ShowSettingsAction = () => Application.Current.Dispatcher.Invoke(() => model.ShowSettings()),
                    ShowCommandsAction = () => Application.Current.Dispatcher.Invoke(() => model.ShowCommands()),
                    ShowModuleSettingsAction = name => Application.Current.Dispatcher.Invoke(() => model.ShowModuleSettings(name)),
                    StartRecordAction = timeout => Application.Current.Dispatcher.Invoke(() => mainService.StartRecord(timeout))
                }
            };
            foreach (var runner in staticRunners)
            {
                ModuleManager.Instance.AddStaticInstance(runner.ShortName, runner);
            }

            #endregion

            await model.Load(isUpdating);

            if (!isUpdating && Settings.Default.AutoUpdateAssemblies)
            {
                Run("update-assemblies");
            }
            if (e.Args.Contains("/run"))
            {
                var commandIndex = e.Args.ToList().IndexOf("/run") + 1;
                var text = e.Args[commandIndex].Trim('"');
                var commands = text.Split(';');

                foreach (var command in commands)
                {
                    Run(command);
                }
            }
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
