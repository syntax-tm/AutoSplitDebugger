using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace AutoSplitDebugger;

// ReSharper disable once BadControlBracesIndent

public class FilteredKeyboardHook : KeyboardHook
{
    public override event HookEventHandler KeyDown;
    public override event HookEventHandler KeyUp;

    private readonly VIRTUAL_KEY _key;
    private readonly bool _shift;
    private readonly bool _control;
    private readonly bool _alt;
    private readonly bool _modifiers;

    private const WINDOWS_HOOK_ID HOOK_TYPE = WINDOWS_HOOK_ID.WH_KEYBOARD_LL;

    public FilteredKeyboardHook(VIRTUAL_KEY key, bool shift = false, bool control = false, bool alt = false)
    {
        _key = key;
        _shift = shift;
        _control = control;
        _alt = alt;
        _modifiers = shift || control || alt;

        Install();
    }

    ~FilteredKeyboardHook()
    {
        Uninstall();
    }

    // hook function called by system
    protected override LRESULT HookCallback(int code, WPARAM wParam, LPARAM lParam)
    {
        if (code < 0)
        {
            return PInvoke.CallNextHookEx(_hookHandle, code, wParam, lParam);
        }

        var kbdStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
        var key = (VIRTUAL_KEY)kbdStruct.vkCode;

        if (key != _key)
        {
            goto NEXT_HOOK;
        }

        // if we require shift, ctrl, or alt to be pressed
        if (_modifiers)
        {
            var modifiers = Control.ModifierKeys;

            var shift = modifiers.HasFlag(Keys.Shift);
            if (_shift && !shift)
            {
                goto NEXT_HOOK;
            }

            var control = modifiers.HasFlag(Keys.Control);
            if (_control && !control)
            {
                goto NEXT_HOOK;
            }

            var alt = modifiers.HasFlag(Keys.Alt);
            if (_alt && !alt)
            {
                goto NEXT_HOOK;
            }
        }

        var isKeyUp = kbdStruct.flags.HasFlag(KBDLLHOOKSTRUCT_FLAGS.LLKHF_UP);

        // KeyUp event
        if (isKeyUp)
        {
            KeyUp?.Invoke(this, new(kbdStruct));
        }

        // KeyDown event
        if (!isKeyUp)
        {
            KeyDown?.Invoke(this, new(kbdStruct));
        }

    NEXT_HOOK:
        return PInvoke.CallNextHookEx(_hookHandle, code, wParam, lParam);
    }
}
