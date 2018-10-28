using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace HomeCenter.NET.Utilities.HookModules
{
    public class HookModule
    {
        public List<Key> ActivationKeys { get; set; }
        public List<ModifierKeys> ActivationModifiers { get; set; }

        public bool IsHookCombination() =>
            ActivationKeys.All(Keyboard.IsKeyDown) && ActivationModifiers.All(i => (Keyboard.Modifiers & i) == i);

        public HookModule(List<Key> keys, List<ModifierKeys> modifiers)
        {
            ActivationKeys = keys ?? new List<Key>();
            ActivationModifiers = modifiers ?? new List<ModifierKeys>();
        }
    }
}
