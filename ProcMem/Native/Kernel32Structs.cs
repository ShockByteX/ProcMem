using System;
using System.Runtime.InteropServices;

namespace ProcMem.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryBasicInformation
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public MemoryProtectionFlags AllocationProtect;
        public int RegionSize;
        public MemoryStateFlags State;
        public MemoryProtectionFlags MemoryProtection;
        public MemoryTypeFlags Type;

        public static readonly int StructSize = Marshal.SizeOf(typeof(MemoryBasicInformation));
    }

    public struct SystemInfo
    {
        public ushort processorArchitecture;
        ushort reserved;
        public uint pageSize;
        public IntPtr minimumApplicationAddress;
        public IntPtr maximumApplicationAddress;
        public IntPtr activeProcessorMask;
        public uint numberOfProcessors;
        public uint processorType;
        public uint allocationGranularity;
        public ushort processorLevel;
        public ushort processorRevision;

        public static readonly int StructSize = Marshal.SizeOf(typeof(SystemInfo));
    }
}