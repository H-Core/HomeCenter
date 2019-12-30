using System;
using H.NET.Core.Managers;
using H.NET.Utilities;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Services;
using HomeCenter.NET.Utilities.HookModules;
using HomeCenter.NET.ViewModels;

namespace HomeCenter.NET.Initializers
{
    public class HookInitializer
    {
        public HookInitializer(BaseManager manager, HookService hookService, MainViewModel model, ScreenshotToClipboardModule screenshotToClipboardModule, ScreenshotToTextModule screenshotToTextModule, Settings settings)
        {
            void GlobalKeyUp(object? sender, KeyboardHookEventArgs e)
            {
                if (e.Key != Keys.None && e.Key == hookService.RecordKey ||
                    e.IsAltPressed && e.IsCtrlPressed)
                {
                    manager.Stop();
                }
            }

            void GlobalKeyDown(object? sender, KeyboardHookEventArgs e)
            {
                 if (e.Key != Keys.None && e.Key == hookService.RecordKey ||
                    e.Key == Keys.Space && e.IsAltPressed && e.IsCtrlPressed)
                {
                    manager.Start();
                }

                if (hookService.IsIgnoredApplication())
                {
                    return;
                }

                //Print($"{e.Key:G}");
                var combination = new KeysCombination(e.Key, e.IsCtrlPressed, e.IsShiftPressed, e.IsAltPressed);
                if (hookService.RunCombination(combination))
                {
                    e.Handled = true;
                }
            }

            void GlobalMouseDown(object? sender, MouseEventExtArgs e)
            {
                if (e.SpecialButton == 0)
                {
                    return;
                }
                if (hookService.IsIgnoredApplication())
                {
                    return;
                }

                var combination = KeysCombination.FromSpecialData(e.SpecialButton);
                if (hookService.RunCombination(combination))
                {
                    e.Handled = true;
                }
                //Print($"{e.SpecialButton}");
            }

            try
            {
                hookService.KeyboardHook.SetEnabled(settings.EnableKeyboardHook);
                hookService.MouseHook.SetEnabled(settings.EnableMouseHook);

                hookService.KeyboardHook.KeyUp += GlobalKeyUp;
                hookService.KeyboardHook.KeyDown += GlobalKeyDown;

                // TODO: mouse move speed change bug
                hookService.MouseHook.MouseUp += screenshotToClipboardModule.Global_MouseUp;
                hookService.MouseHook.MouseDown += screenshotToClipboardModule.Global_MouseDown;
                hookService.MouseHook.MouseMove += screenshotToClipboardModule.Global_MouseMove;

                hookService.MouseHook.MouseUp += screenshotToTextModule.Global_MouseUp;
                hookService.MouseHook.MouseDown += screenshotToTextModule.Global_MouseDown;
                hookService.MouseHook.MouseMove += screenshotToTextModule.Global_MouseMove;

                hookService.MouseHook.MouseDown += GlobalMouseDown;
            }
            catch (Exception exception)
            {
                model.Print(exception.ToString());
            }
        }
    }
}
