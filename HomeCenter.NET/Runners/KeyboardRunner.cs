using System;
using System.Linq;
using WindowsInput;
using WindowsInput.Native;
using H.NET.Core.Runners;

namespace HomeCenter.NET.Runners
{
    public class KeyboardRunner : Runner
    {
        #region Constructors

        public KeyboardRunner()
        {
            AddAction("keyboard", KeyboardCommand, "CONTROL+V");
        }

        #endregion

        #region Private methods

        private static VirtualKeyCode ToVirtualKey(string key)
        {
            string text;
            if (key.Length == 1)
            {
                text = $"VK_{key}";
            }
            else if (key.ToLowerInvariant() == "alt")
            {
                text = "MENU";
            }
            else if (key.ToLowerInvariant() == "ctrl")
            {
                text = "CONTROL";
            }
            else if (key.ToLowerInvariant() == "enter")
            {
                text = "RETURN";
            }
            else
            {
                text = key;
            }
            
            return (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), text, true);
        } 

        private static void KeyboardCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            var keys = command.Split('+');
            var mainKey = ToVirtualKey(keys.LastOrDefault());
            var otherKeys = keys.Take(keys.Length - 1).Select(ToVirtualKey);
            
            new InputSimulator().Keyboard.ModifiedKeyStroke(otherKeys, mainKey);
        }

        #endregion
    }
}
