using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace AutoSplitDebugger.Core.Native;

public class KeyboardHook
{
    private const WINDOWS_HOOK_ID HOOK_TYPE = WINDOWS_HOOK_ID.WH_KEYBOARD_LL;

    public delegate void HookEventHandler(object sender, HookEventArgs e);

    public virtual event HookEventHandler KeyDown;
    public virtual event HookEventHandler KeyUp;

    protected readonly HOOKPROC _hookFunction;
    protected UnhookWindowsHookExSafeHandle _hookHandle;

    // ReSharper disable once MemberCanBeProtected.Global
    public KeyboardHook()
    {
        _hookFunction = HookCallback;

        Install();
    }

    ~KeyboardHook()
    {
        Uninstall();
    }

    // hook function called by system
    protected virtual LRESULT HookCallback(int code, WPARAM wParam, LPARAM lParam)
    {
        if (code < 0)
        {
            return PInvoke.CallNextHookEx(_hookHandle, code, wParam, lParam);
        }

        KBDLLHOOKSTRUCT kbdStruct = new();

        Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

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

        return PInvoke.CallNextHookEx(_hookHandle, code, wParam, lParam);
    }

    protected void Install()
    {
        // make sure not already installed
        if (_hookHandle != null)
            return;

        // need instance handle to module to create a system-wide hook
        var modules = Assembly.GetExecutingAssembly().GetModules();

        Debug.Assert(modules is { Length: > 0 });

        var hModule = PInvoke.GetModuleHandle(modules[0].Name);

        // install system-wide hook
        _hookHandle = PInvoke.SetWindowsHookEx(HOOK_TYPE, _hookFunction, hModule, 0);
    }

    protected void Uninstall()
    {
        if (_hookHandle == null) return;

        var hHook = (HHOOK)_hookHandle.DangerousGetHandle();

        // uninstall system-wide hook
        PInvoke.UnhookWindowsHookEx(hHook);
        _hookHandle.Close();
    }
}