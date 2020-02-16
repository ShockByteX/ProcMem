using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProcMem.Memory;
using ProcMem.Native;

namespace ProcMem.Utilities
{
    public static unsafe class MemoryHelper
    {
        public static byte[] ReadProcessMemory(IntPtr hProcess, IntPtr address, int size)
        {
            var data = new byte[size];

            if (Kernel32.ReadProcessMemory(hProcess, address, data, size, out int nbBytesRead) && size == nbBytesRead)
            {
                return data;
            }

            throw new Win32Exception($"Couldn't read from 0x{address.ToInt64():X}.");
        }
        public static int WriteProcessMemory(IntPtr hProcess, IntPtr address, byte[] data)
        {
            Kernel32.VirtualProtectEx(hProcess, address, data.Length, MemoryProtectionFlags.ExecuteReadWrite, out var oldProtection);

            if (Kernel32.WriteProcessMemory(hProcess, address, data, data.Length, out int nbBytesWritten) && nbBytesWritten == data.Length)
            {
                Kernel32.VirtualProtectEx(hProcess, address, data.Length, oldProtection, out oldProtection);
                return nbBytesWritten;
            }

            Kernel32.VirtualProtectEx(hProcess, address, data.Length, oldProtection, out oldProtection);
            throw new Win32Exception($"Couldn't write {data.Length} bytes to 0x{address.ToInt64():X}");
        }

        public static byte[] ReadMemory(IntPtr address, int length)
        {
            var data = new byte[length];

            fixed (byte* ptrData = data)
            {
                Msvcrt.memcpy((IntPtr)ptrData, address, length);
            }

            return data;
        }

        public static int WriteMemory(IntPtr hProcess, IntPtr address, byte[] data)
        {
            Kernel32.VirtualProtectEx(hProcess, address, data.Length, MemoryProtectionFlags.ExecuteReadWrite, out MemoryProtectionFlags oldProtection);

            fixed (byte* ptrData = data)
            {
                Msvcrt.memcpy(address, (IntPtr)ptrData, data.Length);
            }

            Kernel32.VirtualProtectEx(hProcess, address, data.Length, oldProtection, out oldProtection);
            return data.Length;
        }

        public static MemoryProtectionFlags GetMemoryProtection(IntPtr hProcess, IntPtr address)
        {
            Kernel32.VirtualQueryEx(hProcess, address, out var memoryInfo, MemoryBasicInformation.StructSize);
            return memoryInfo.MemoryProtection;
        }

        public static MemoryBasicInformation Query(IntPtr hProcess, IntPtr address)
        {
            if (Kernel32.VirtualQueryEx(hProcess, address, out var memoryInfo, MemoryBasicInformation.StructSize) != 0)
            {
                return memoryInfo;
            }
            throw new Win32Exception($"Couldn't query information about the memory region 0x{address.ToInt64():X}");
        }
    }
}