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
        public long RegionSize;
        public MemoryStateFlags State;
        public MemoryProtectionFlags MemoryProtection;
        public MemoryTypeFlags Type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryBasicInformation32
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public MemoryProtectionFlags AllocationProtect;
        public int RegionSize;
        public MemoryStateFlags State;
        public MemoryProtectionFlags MemoryProtection;
        public MemoryTypeFlags Type;

        public static readonly int StructSize = Marshal.SizeOf(typeof(MemoryBasicInformation32));

        public static implicit operator MemoryBasicInformation(MemoryBasicInformation32 memInfo)
        {
            return new MemoryBasicInformation()
            {
                BaseAddress = memInfo.BaseAddress,
                AllocationBase = memInfo.AllocationBase,
                AllocationProtect = memInfo.AllocationProtect,
                RegionSize = memInfo.RegionSize,
                State = memInfo.State,
                MemoryProtection = memInfo.MemoryProtection,
                Type = memInfo.Type
            };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryBasicInformation64
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public MemoryProtectionFlags AllocationProtect;
        public uint Alignment1;
        public long RegionSize;
        public MemoryStateFlags State;
        public MemoryProtectionFlags MemoryProtection;
        public MemoryTypeFlags Type;
        public uint Alignment2;

        public static readonly int StructSize = Marshal.SizeOf(typeof(MemoryBasicInformation64));

        public static implicit operator MemoryBasicInformation(MemoryBasicInformation64 memInfo)
        {
            return new MemoryBasicInformation()
            {
                BaseAddress = memInfo.BaseAddress,
                AllocationBase = memInfo.AllocationBase,
                AllocationProtect =  memInfo.AllocationProtect,
                RegionSize = memInfo.RegionSize,
                State = memInfo.State,
                MemoryProtection = memInfo.MemoryProtection,
                Type = memInfo.Type
            };
        }
    }

    public struct SystemInfo
    {
        public ushort ProcessorArchitecture;
        public ushort Reserved;
        public uint PgeSize;
        public IntPtr MinimumApplicationAddress;
        public IntPtr MaximumApplicationAddress;
        public IntPtr ActiveProcessorMask;
        public uint NumberOfProcessors;
        public uint ProcessorType;
        public uint AllocationGranularity;
        public ushort ProcessorLevel;
        public ushort ProcessorRevision;

        public static readonly int StructSize = Marshal.SizeOf(typeof(SystemInfo));
    }
}