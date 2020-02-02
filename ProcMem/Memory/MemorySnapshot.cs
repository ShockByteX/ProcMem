using System;

namespace ProcMem.Memory
{
    public class MemorySnapshot : IEquatable<MemorySnapshot>
    {
        public readonly IntPtr Address;
        public readonly byte[] Data;

        public MemorySnapshot(IntPtr address, byte[] data)
        {
            Address = address;
            Data = data;
        }

        public bool Equals(MemorySnapshot other)
        {
            if (other is null) return false;
            return Address.Equals(other.Address) && Data.Length.Equals(other.Data.Length);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is MemorySnapshot snapshot) return Equals(snapshot);
            return false;
        }
        public override int GetHashCode() => Address.GetHashCode() ^ Data.Length.GetHashCode();
        public override string ToString() => $"Address: 0x{Address.ToInt64():X}, Size: {Data.Length}";

        public static MemorySnapshot Create(IMemory memory, IntPtr address, int size)
        {
            var data = memory.Read(address, size);
            return new MemorySnapshot(address, data);
        }
    }
}