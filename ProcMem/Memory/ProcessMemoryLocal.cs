using System;
using System.Runtime.InteropServices;
using ProcMem.Extensions;
using ProcMem.Utilities;

namespace ProcMem.Memory
{
    public class ProcessMemoryLocal : ProcessMemory
    {
        public ProcessMemoryLocal(IntPtr handle) : base(handle) { }

        public override byte[] Read(IntPtr address, int length) => MemoryHelper.ReadMemory(address, length);
        public override T Read<T>(IntPtr address) => address.Read<T>();

        public override int Write(IntPtr address, byte[] data) => MemoryHelper.WriteMemory(Handle, address, data);
        public override void Write<T>(IntPtr address, T value) => Write(address, value, false);
        public void Write<T>(IntPtr address, T value, bool replace) => Marshal.StructureToPtr(value, address, replace);
    }
}