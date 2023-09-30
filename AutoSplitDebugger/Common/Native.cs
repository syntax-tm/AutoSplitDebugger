using System;
using System.Runtime.InteropServices;

namespace AutoSplitDebugger;

[Flags]
public enum ThreadAccess : int
{
    TERMINATE = 0x0001,
    SUSPEND_RESUME = 0x0002,
    GET_CONTEXT = 0x0008,
    SET_CONTEXT = 0x0010,
    SET_INFORMATION = 0x0020,
    QUERY_INFORMATION = 0x0040,
    SET_THREAD_TOKEN = 0x0080,
    IMPERSONATE = 0x0100,
    DIRECT_IMPERSONATION = 0x0200
}

public static partial class Native
{
    public const uint PROCESS_ALL_ACCESS = 0x1fffff;
    public const uint PROCESS_SUSPEND_RESUME = 0x0800;
    public const uint PROCESS_QUERY_INFORMATION = 0x0400;
    public const uint MEM_COMMIT = 0x00001000;
    public const uint PAGE_READWRITE = 0x04;
    public const uint PROCESS_WM_READ = 0x0010;

    [LibraryImport("kernel32.dll")]
    public static partial nint OpenProcess(int dwDesiredAccess, [MarshalAs(UnmanagedType.U1)] bool bInheritHandle, int dwProcessId);
        
    [LibraryImport("ntdll", SetLastError = true)]
    [return:MarshalAs(UnmanagedType.I1)]  
    public static partial bool NtReadVirtualMemory(
        nint processHandle,
        nint baseAddress,
        byte[] buffer,
        int numberOfBytesToRead,
        int numberOfBytesRead);

    [LibraryImport("ntdll", SetLastError = true)]
    [return:MarshalAs(UnmanagedType.I1)]  
    public static partial bool NtWriteVirtualMemory(
        nint processHandle,
        nint baseAddress,
        byte[] buffer,
        int numberOfBytesToWrite,
        int numberOfBytesWritten);

    [LibraryImport("ntdll.dll", EntryPoint = "NtSuspendProcess", SetLastError = true)]
    public static partial nuint NtSuspendProcess(nuint processHandle);

    [LibraryImport("ntdll.dll", EntryPoint = "NtResumeProcess", SetLastError = true)]
    public static partial nuint NtResumeProcess(nuint processHandle);

    [LibraryImport("kernel32.dll")]
    public static partial nint OpenThread(ThreadAccess dwDesiredAccess, [MarshalAs(UnmanagedType.I1)] bool bInheritHandle, uint dwThreadId);

    [LibraryImport("kernel32.dll")]
    public static partial uint SuspendThread(nint hThread);

    [LibraryImport("kernel32.dll")]
    public static partial int ResumeThread(nint hThread);

    [LibraryImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool CloseHandle(nint handle);
}