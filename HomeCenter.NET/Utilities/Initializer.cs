﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using H.NET.Core;
using H.NET.Core.Runners;
using H.NET.Plugins;
using H.NET.Utilities;
using HomeCenter.NET.Extensions;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Runners;
using HomeCenter.NET.Services;
using HomeCenter.NET.ViewModels;

namespace HomeCenter.NET.Utilities
{
    public static class Initializer
    {
        internal static async Task InitializeDynamicModules(MainService mainService, HookService hookService, MainViewModel model)
        {
            #region Modules

            AssembliesManager.LogAction = model.Print;
            Module.LogAction = model.Print;
            Runner.SearchFunc = model.Search;

            model.Print("Loading modules...");
            try
            {
                await mainService.Load();

                model.Print("Loaded");
            }
            catch (Exception exception)
            {
                model.Print(exception.ToString());
            }

            #endregion
        }

        internal static void InitializeHooks(MainService mainService, HookService hookService, MainViewModel model)
        {
            void GlobalKeyUp(object sender, KeyboardHookEventArgs e)
            {
                if (e.Key != Keys.None && e.Key == Options.RecordKey ||
                    e.IsAltPressed && e.IsCtrlPressed)
                {
                    mainService.Manager.Stop();
                }
            }

            void GlobalKeyDown(object sender, KeyboardHookEventArgs e)
            {
                if (e.Key != Keys.None && e.Key == Options.RecordKey ||
                    e.Key == Keys.Space && e.IsAltPressed && e.IsCtrlPressed)
                {
                    mainService.Manager.Start();
                }

                if (Options.IsIgnoredApplication())
                {
                    return;
                }

                //Print($"{e.Key:G}");
                var combination = new KeysCombination(e.Key, e.IsCtrlPressed, e.IsShiftPressed, e.IsAltPressed);
                if (mainService.RunCombination(combination))
                {
                    e.Handled = true;
                }
            }

            void GlobalMouseDown(object sender, MouseEventExtArgs e)
            {
                if (e.SpecialButton == 0)
                {
                    return;
                }
                if (Options.IsIgnoredApplication())
                {
                    return;
                }

                var combination = KeysCombination.FromSpecialData(e.SpecialButton);
                if (mainService.RunCombination(combination))
                {
                    e.Handled = true;
                }
                //Print($"{e.SpecialButton}");
            }

            try
            {
                hookService.KeyboardHook.SetEnabled(Settings.Default.EnableKeyboardHook);
                hookService.MouseHook.SetEnabled(Settings.Default.EnableMouseHook);

                hookService.KeyboardHook.KeyUp += GlobalKeyUp;
                hookService.KeyboardHook.KeyDown += GlobalKeyDown;

                ScreenshotRectangle.ActivationKeys.Add(Key.Space);
                ScreenshotRectangle.ActivationModifiers.Add(ModifierKeys.Shift);
                ScreenshotRectangle.NewImage += image => Clipboard.SetImage(image.ToBitmapImage());
                hookService.MouseHook.MouseUp += ScreenshotRectangle.Global_MouseUp;
                hookService.MouseHook.MouseDown += ScreenshotRectangle.Global_MouseDown;
                hookService.MouseHook.MouseMove += ScreenshotRectangle.Global_MouseMove;

                hookService.MouseHook.MouseDown += GlobalMouseDown;
            }
            catch (Exception exception)
            {
                model.Print(exception.ToString());
            }
        }

        internal static void InitializeStaticRunners(MainViewModel model, MainService mainService)
        {
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
                    ShowSettingsAction = () => Application.Current.Dispatcher.Invoke(model.ShowSettings),
                    ShowCommandsAction = () => Application.Current.Dispatcher.Invoke(model.ShowCommands),
                    ShowModuleSettingsAction = name => Application.Current.Dispatcher.Invoke(() => model.ShowModuleSettings(name)),
                    StartRecordAction = timeout => Application.Current.Dispatcher.Invoke(() => mainService.StartRecord(timeout))
                }
            };
            foreach (var runner in staticRunners)
            {
                ModuleManager.Instance.AddStaticInstance(runner.ShortName, runner);
            }
        }

        public static void CheckKillAll(string[] args)
        {
            var isKillAll = args.Contains("/killall");
            if (isKillAll)
            {
                Process.GetProcessesByName(Options.ApplicationName)
                    .Where(i => i.Id != Process.GetCurrentProcess().Id)
                    .AsParallel()
                    .ForAll(i => i.Kill());
            }
        }

        public static void CheckNotFirstProcess(string[] args)
        {
            var isKillAll = args.Contains("/killall");
            var isRestart = args.Contains("/restart");
            if (Process.GetProcessesByName(Options.ApplicationName).Length > 1 &&
                !isRestart && !isKillAll)
            {
                Application.Current.Shutdown();
            }
        }

        public static void CheckUpdate(string[] args, MainService mainService)
        {
            var isUpdating = args.Contains("/updating");
            if (!isUpdating && Settings.Default.AutoUpdateAssemblies)
            {
                mainService.Run("update-assemblies");
            }
        }

        public static void CheckRun(string[] args, MainService mainService)
        {
            if (args.Contains("/run"))
            {
                var commandIndex = args.ToList().IndexOf("/run") + 1;
                var text = args[commandIndex].Trim('"');
                var commands = text.Split(';');

                foreach (var command in commands)
                {
                    mainService.Run(command);
                }
            }
        }
    }
}