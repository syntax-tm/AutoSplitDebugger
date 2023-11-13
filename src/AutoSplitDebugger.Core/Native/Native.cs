using System.Runtime.InteropServices;

namespace AutoSplitDebugger.Core.Native;

public static partial class Native
{
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
}