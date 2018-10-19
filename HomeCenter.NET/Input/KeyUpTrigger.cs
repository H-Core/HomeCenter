using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace HomeCenter.NET.Input
{
    public class KeyUpTrigger : TriggerBase<UIElement>
    {
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(KeyUpTrigger), null);

        public static readonly DependencyProperty ModifiersProperty =
            DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(KeyUpTrigger), null);

        public static KeyUpTrigger FromText(string text)
        {
            var isGesture = text.Contains("+");
            if (!isGesture)
            {
                var key = (Key)Enum.Parse(typeof(Key), text, true);

                return new KeyUpTrigger { Key = key };
            }

            var mkg = (MultiKeyGesture)(new MultiKeyGestureConverter()).ConvertFrom(text);
            if (mkg == null)
            {
                throw new ArgumentException("MultiKeyGesture is null");
            }

            return new KeyUpTrigger { Modifiers = mkg.KeySequences[0].Modifiers, Key = mkg.KeySequences[0].Keys[0] };
        }

        public Key Key
        {
            get => (Key)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        public ModifierKeys Modifiers
        {
            get => (ModifierKeys)GetValue(ModifiersProperty);
            set => SetValue(ModifiersProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.KeyUp += OnAssociatedObjectKeyUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.KeyUp -= OnAssociatedObjectKeyUp;
        }

        private void OnAssociatedObjectKeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key) && (Keyboard.Modifiers == GetActualModifiers(e.Key, Modifiers)))
            {
                InvokeActions(e);
            }
        }

        static ModifierKeys GetActualModifiers(Key key, ModifierKeys modifiers)
        {
            switch (key)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    modifiers |= ModifierKeys.Control;
                    return modifiers;

                case Key.LeftAlt:
                case Key.RightAlt:
                    modifiers |= ModifierKeys.Alt;
                    return modifiers;

                case Key.LeftShift:
                case Key.RightShift:
                    modifiers |= ModifierKeys.Shift;
                    break;
            }

            return modifiers;
        }
    }
}