using System;
using System.Windows.Forms;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace AutoSplitDebugger.Core.Native;

public class HookEventArgs : EventArgs
{
    public VIRTUAL_KEY Key;
    public KBDLLHOOKSTRUCT_FLAGS Flags;
    public bool Shift;
    public bool Control;
    public bool Alt;

    public HookEventArgs(KBDLLHOOKSTRUCT param)
    {
        Key = (VIRTUAL_KEY)param.vkCode;

        // detect what modifier keys are pressed, using 
        // Windows.Forms.Control.ModifierKeys instead of Keyboard.Modifiers
        // since Keyboard.Modifiers does not correctly get the state of the 
        // modifier keys when the application does not have focus
        var modifiers = System.Windows.Forms.Control.ModifierKeys;

        Shift = modifiers.HasFlag(Keys.Shift);
        Control = modifiers.HasFlag(Keys.Control);
        Alt = modifiers.HasFlag(Keys.Alt);
    }
}
