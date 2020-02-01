using System;

namespace ProcMem.Memory
{
    public class MemoryObject<T> : MemoryPointer
    {
        public MemoryObject(IMemory memory, IntPtr address) : base(memory, address) { }

        public T Value
        {
            get => Memory.Read<T>(Address);
            set => Memory.Write(Address, value);
        }
    }
}