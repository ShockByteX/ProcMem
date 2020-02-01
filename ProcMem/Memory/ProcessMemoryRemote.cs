using System;
using ProcMem.Marshaling;
using ProcMem.Utilities;

namespace ProcMem.Memory
{
    public class ProcessMemoryRemote : ProcessMemory
    {
        public ProcessMemoryRemote(IntPtr handle) : base(handle) { }

        public override byte[] Read(IntPtr address, int length) => MemoryHelper.ReadProcessMemory(Handle, address, length);
        public override T Read<T>(IntPtr address) => MarshalType<T>.ByteArrayToObject(Read(address, MarshalType<T>.Size));

        public override int Write(IntPtr address, byte[] data) => MemoryHelper.WriteProcessMemory(Handle, address, data);
        public override void Write<T>(IntPtr address, T value) => Write(address, MarshalType<T>.ObjectToByteArray(value));
    }
}