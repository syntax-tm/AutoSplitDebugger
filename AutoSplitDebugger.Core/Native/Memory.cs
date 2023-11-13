using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.System.Threading;
using AutoSplitDebugger.Core.Models;
using log4net;

namespace AutoSplitDebugger.Core.Native;

public class Memory
{
    private const PROCESS_ACCESS_RIGHTS OPEN_PROCESS_FLAGS = PROCESS_ACCESS_RIGHTS.PROCESS_VM_OPERATION | PROCESS_ACCESS_RIGHTS.PROCESS_VM_READ | PROCESS_ACCESS_RIGHTS.PROCESS_VM_WRITE;
    private const int BYTE_ALIGNMENT = 4;

    private readonly ILog log = LogManager.GetLogger(nameof(Memory));

    private string _processName;
    private nint _processHandle;
    private readonly Dictionary<int, uint> _threadStates = new ();

#pragma warning disable 649
    private readonly int m_iBytesWritten;
    private readonly int m_iBytesRead;
#pragma warning restore 649

    public Process Process { get; private set; }

    public ProcessModule MainModule => Process.MainModule;
    public nint BaseAddress => MainModule.BaseAddress;

    public bool IsAttached { get; private set; }
    public ModuleVersionInfo ModuleVersionInfo { get; private set; }
    
    public Memory()
    {

    }

    public Memory(string processName)
    {
        if (string.IsNullOrWhiteSpace(processName)) throw new ArgumentNullException(processName);

        _processName = processName;
    }

    public bool Attach(IEnumerable<ModuleVersionInfo> moduleVersions)
    {
        if (moduleVersions == null) throw new ArgumentNullException(nameof(moduleVersions));
        try
        {
            foreach (var versionInfo in moduleVersions)
            {
                // initial check is just done on the process name
                var processes = Process.GetProcessesByName(versionInfo.ModuleName);

                if (!processes.Any()) continue;

                // verify this is the correct module size etc
                if (!versionInfo.IsMatch(processes[0])) continue;

                Process = processes[0];
                ModuleVersionInfo = versionInfo;
                _processName = Process.ProcessName;

                break;
            }

            if (Process == null) return false;
            
            _processHandle = PInvoke.OpenProcess(OPEN_PROCESS_FLAGS, false, (uint) Process.Id);

            IsAttached = true;

            return true;
        }
        catch (Exception e)
        {
            log.Error($"An error occurred attempting to attach to process. {e.Message}", e);

            return false;
        }
    }

    public bool IsSuspended()
    {
        return _threadStates.Values.Any(v => v > 0);
    }

    public void SuspendProcess()
    {
        foreach (ProcessThread pT in Process.Threads)
        {
            var pOpenThread = PInvoke.OpenThread(THREAD_ACCESS_RIGHTS.THREAD_SUSPEND_RESUME, false, (uint) pT.Id);
            if (pOpenThread == nint.Zero)
            {
                var openEx = GetLastErrorAsException();
                throw openEx;
            }

            var suspendResult = PInvoke.SuspendThread(pOpenThread);
            if (suspendResult == uint.MaxValue)
            {
                var suspendEx = GetLastErrorAsException();
                throw suspendEx;
            }

            _threadStates[pT.Id] = suspendResult;
            
            if (PInvoke.CloseHandle(pOpenThread))
            {
                return;
            }
            
            var closeEx = GetLastErrorAsException();
            throw closeEx;
        }
    }

    public void ResumeProcess()
    {
        foreach (ProcessThread pT in Process.Threads)
        {
            var pOpenThread = PInvoke.OpenThread(THREAD_ACCESS_RIGHTS.THREAD_SUSPEND_RESUME, false, (uint) pT.Id);
            if (pOpenThread == nint.Zero)
            {
                continue;
            }

            var suspendResult = PInvoke.ResumeThread(pOpenThread);
            if (suspendResult == uint.MaxValue)
            {
                var resumeEx = GetLastErrorAsException();
                throw resumeEx;
            }

            // 0 when thread was not suspended
            // 1 when thread was suspended, but restarted
            // else the thread is still suspended
            _threadStates[pT.Id] = suspendResult > 1
                ? suspendResult
                : 0;
            
            if (PInvoke.CloseHandle(pOpenThread))
            {
                return;
            }

            var closeEx = GetLastErrorAsException();
            throw closeEx;
        }
    }

    public void WriteMemory(int address, object value)
    {
        var buffer = StructureToByteArray(value);
        var ptr = new nint(address);

        Native.NtWriteVirtualMemory(_processHandle, ptr, buffer, buffer.Length, m_iBytesWritten);
    }
        
    public void WriteMemory(nint ptr, object value)
    {
        var buffer = StructureToByteArray(value);

        Native.NtWriteVirtualMemory(_processHandle, ptr, buffer, buffer.Length, m_iBytesWritten);
    }

    public void WriteMemory(int address, char[] value)
    {
        var buffer = Encoding.UTF8.GetBytes(value);
        var ptr = new nint(address);

        Native.NtWriteVirtualMemory(_processHandle, ptr, buffer, buffer.Length, m_iBytesWritten);
    }
        
    public void WriteMemory(nint ptr, char[] value)
    {
        var buffer = Encoding.UTF8.GetBytes(value);

        Native.NtWriteVirtualMemory(_processHandle, ptr, buffer, buffer.Length, m_iBytesWritten);
    }
        
    public T ReadMemory<T>(params int[] offsets) where T : struct
    {
        var byteSize = Marshal.SizeOf(typeof(T));

        var buffer = new byte[byteSize];
        var address = ResolvePath(offsets);

        Native.NtReadVirtualMemory(_processHandle, address, buffer, buffer.Length, m_iBytesRead);

        return ByteArrayToStructure<T>(buffer);
    }

    public T ReadMemory<T>(int address) where T : struct
    {
        var byteSize = Marshal.SizeOf(typeof(T));

        var buffer = new byte[byteSize];
        var ptr = new nint(address);

        Native.NtReadVirtualMemory(_processHandle, ptr, buffer, buffer.Length, m_iBytesRead);

        return ByteArrayToStructure<T>(buffer);
    }
        
    public T ReadMemory<T>(nint ptr) where T : struct
    {
        var byteSize = Marshal.SizeOf(typeof(T));

        var buffer = new byte[byteSize];

        Native.NtReadVirtualMemory(_processHandle, ptr, buffer, buffer.Length, m_iBytesRead);

        return ByteArrayToStructure<T>(buffer);
    }

    public byte[] ReadMemory(int offset, int size)
    {
        var buffer = new byte[size];
        var ptr = new nint(offset);

        Native.NtReadVirtualMemory(_processHandle, ptr, buffer, size, m_iBytesRead);

        return buffer;
    }
        
    public byte[] ReadMemory(nint ptr, int size)
    {
        var buffer = new byte[size];

        Native.NtReadVirtualMemory(_processHandle, ptr, buffer, size, m_iBytesRead);

        return buffer;
    }

    public nint ResolvePath(params int[] offsets)
    {
        var current = BaseAddress;
        var last = offsets.Last();

        foreach (var offset in offsets.SkipLast(1))
        {
            var nextAddress = current + offset;
            var next = ReadMemory<int>(nextAddress);
            current = new (next);
        }

        current = nint.Add(current, last);
            
        return current;
    }

    private static Win32Exception GetLastErrorAsException()
    {
        var error = Marshal.GetLastPInvokeError();

        return new (error);
    }

#region Conversion

    public static float[] ConvertToFloatArray(byte[] bytes)
    {
        if (bytes.Length % BYTE_ALIGNMENT != 0) throw new ArgumentException($"{nameof(bytes)} is aligned properly.");

        var floats = new float[bytes.Length / BYTE_ALIGNMENT];

        for (var i = 0; i < floats.Length; i++)
        {
            floats[i] = BitConverter.ToSingle(bytes, i * BYTE_ALIGNMENT);
        }

        return floats;
    }

    public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            var value = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            if (value == null) throw new InvalidOperationException();
            return (T) value;
        }
        finally
        {
            handle.Free();
        }
    }

    public static byte[] StructureToByteArray(object obj)
    {
        var length = Marshal.SizeOf(obj);
        var array = new byte[length];
        var pointer = Marshal.AllocHGlobal(length);

        Marshal.StructureToPtr(obj, pointer, true);
        Marshal.Copy(pointer, array, 0, length);
        Marshal.FreeHGlobal(pointer);

        return array;
    }

#endregion
}