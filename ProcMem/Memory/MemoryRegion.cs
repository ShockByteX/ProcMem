using System;
using System.Collections.Generic;
using ProcMem.Native;
using ProcMem.Utilities;

namespace ProcMem.Memory
{
    public class MemoryRegion : MemoryPointer, IEquatable<MemoryRegion>
    {
        public MemoryRegion(IMemory memory, IntPtr address) : base(memory, address) { }

        public override bool IsValid => base.IsValid && Information.State != MemoryStateFlags.Free;
        public MemoryBasicInformation Information => MemoryHelper.Query(Memory.Handle, Address);

        public IEnumerable<IntPtr> ScanSignature(string pattern, int extra, int offset, bool relative)
        {
            return ScanSignature(pattern,extra, offset, relative, Information.RegionSize);
        }

        public bool Equals(MemoryRegion other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Address.Equals(other.Address) && Information.RegionSize.Equals(other.Information.RegionSize);
        }

        public override string ToString() => $"Address: 0x{Address.ToInt64():X}, Size: 0x{Information.RegionSize:X}, Protection: {Information.MemoryProtection}";
        public override int GetHashCode() => Address.GetHashCode() ^ Information.RegionSize.GetHashCode();
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj is MemoryRegion region) return Equals(region);
            return false;
        }

        public static bool operator ==(MemoryRegion left, MemoryRegion right) => Equals(left, right);
        public static bool operator !=(MemoryRegion left, MemoryRegion right) => !Equals(left, right);
    }
}