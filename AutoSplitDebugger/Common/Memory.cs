﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using log4net;
using static System.Runtime.InteropServices.Marshal;

namespace AutoSplitDebugger;

public class Memory
{
    private const int OPEN_PROCESS_FLAGS = Flags.ProcessVmOperation | Flags.ProcessVmRead | Flags.ProcessVmWrite;

    private readonly ILog log = LogManager.GetLogger(nameof(Memory));

    private readonly string _processName;
    private nint _processHandle;

#pragma warning disable 649
    private readonly int m_iBytesWritten;
    private readonly int m_iBytesRead;
#pragma warning restore 649

    public Process Process { get; private set; }

    public ProcessModule MainModule => Process.MainModule;
    public nint BaseAddress => MainModule.BaseAddress;

    public bool IsAttached { get; private set; }

    public Memory(string processName)
    {
        if (string.IsNullOrWhiteSpace(processName)) throw new ArgumentNullException(processName);

        _processName = processName;
    }
        
    public bool Attach()
    {
        try
        {
            var processes = Process.GetProcessesByName(_processName);

            if (!processes.Any()) return false;

            Process = processes[0];
            _processHandle = Native.OpenProcess(OPEN_PROCESS_FLAGS, false, Process.Id);

            IsAttached = true;

            return true;
        }
        catch (Exception e)
        {
            log.Error($"An error occurred attempting to attach to process. {e.Message}", e);

            return false;
        }
    }

    public void SuspendProcess()
    {
        foreach (ProcessThread pT in Process.Threads)
        {
            var pOpenThread = Native.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

            if (pOpenThread == nint.Zero)
            {
                continue;
            }

            Native.SuspendThread(pOpenThread);

            Native.CloseHandle(pOpenThread);
        }
    }

    public void ResumeProcess()
    {
        foreach (ProcessThread pT in Process.Threads)
        {
            var pOpenThread = Native.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

            if (pOpenThread == nint.Zero)
            {
                continue;
            }

            var suspendCount = 0;
            do
            {
                suspendCount = Native.ResumeThread(pOpenThread);
            } while (suspendCount > 0);

            Native.CloseHandle(pOpenThread);
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
        var byteSize = SizeOf(typeof(T));

        var buffer = new byte[byteSize];
        var address = ResolvePath(offsets);

        Native.NtReadVirtualMemory(_processHandle, address, buffer, buffer.Length, m_iBytesRead);

        return ByteArrayToStructure<T>(buffer);
    }

    public T ReadMemory<T>(int address) where T : struct
    {
        var byteSize = SizeOf(typeof(T));

        var buffer = new byte[byteSize];
        var ptr = new nint(address);

        Native.NtReadVirtualMemory(_processHandle, ptr, buffer, buffer.Length, m_iBytesRead);

        return ByteArrayToStructure<T>(buffer);
    }
        
    public T ReadMemory<T>(nint ptr) where T : struct
    {
        var byteSize = SizeOf(typeof(T));

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

#region Other

    internal struct Flags
    {
        public const int ProcessVmOperation = 0x0008;
        public const int ProcessVmRead = 0x0010;
        public const int ProcessVmWrite = 0x0020;
    }

#endregion

#region Conversion

    public static float[] ConvertToFloatArray(byte[] bytes)
    {
        if (bytes.Length % 4 != 0)
            throw new ArgumentException($"{nameof(bytes)} is aligned properly.");

        var floats = new float[bytes.Length / 4];

        for (var i = 0; i < floats.Length; i++)
        {
            floats[i] = BitConverter.ToSingle(bytes, i * 4);
        }

        return floats;
    }

    public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            var value = PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
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
        var length = SizeOf(obj);
        var array = new byte[length];
        var pointer = AllocHGlobal(length);

        StructureToPtr(obj, pointer, true);
        Copy(pointer, array, 0, length);
        FreeHGlobal(pointer);

        return array;
    }

#endregion
}