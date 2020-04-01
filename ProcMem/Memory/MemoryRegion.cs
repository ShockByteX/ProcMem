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

        public MemoryBasicInformation Information
        {
            get
            {
                MemoryHelper.Query(Memory.Handle, Address, out var memoryInfo);
                return memoryInfo;
            }
           
        }

        public bool Readable =>
            Information.MemoryProtection == MemoryProtectionFlags.ReadOnly ||
            Information.MemoryProtection == MemoryProtectionFlags.ReadWrite ||
            Information.MemoryProtection == MemoryProtectionFlags.ExecuteRead ||
            Information.MemoryProtection == MemoryProtectionFlags.ExecuteReadWrite;

        public bool Writable =>
            Information.MemoryProtection == MemoryProtectionFlags.ReadWrite ||
            Information.MemoryProtection == MemoryProtectionFlags.WriteCopy ||
            Information.MemoryProtection == MemoryProtectionFlags.ExecuteReadWrite ||
            Information.MemoryProtection == MemoryProtectionFlags.ExecuteWriteCopy ||
            Information.MemoryProtection == MemoryProtectionFlags.WriteCombine;

        public bool Executable =>
            Information.MemoryProtection == MemoryProtectionFlags.Execute ||
            Information.MemoryProtection == MemoryProtectionFlags.ExecuteRead ||
            Information.MemoryProtection == MemoryProtectionFlags.ExecuteReadWrite ||
            Information.MemoryProtection == MemoryProtectionFlags.ExecuteWriteCopy ||
            Information.MemoryProtection == MemoryProtectionFlags.WriteCombine;

        public bool Guarded => Information.MemoryProtection.HasFlag(MemoryProtectionFlags.Guard);

        public bool Valid => (Readable | Writable | Executable) && Guarded;

        public IEnumerable<IntPtr> ScanSignature(string pattern, int offset, int extra, bool relative, bool firstOnly)
        {
            return ScanSignature(pattern, (int)Information.RegionSize, offset, extra, relative, firstOnly);
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
            switch (obj)
            {
                case null: 
                    return false;
                case MemoryRegion region: 
                    return Equals(region);
                default:
                    return false;
            }
        }

        public static bool operator ==(MemoryRegion left, MemoryRegion right) => Equals(left, right);
        public static bool operator !=(MemoryRegion left, MemoryRegion right) => !Equals(left, right);
    }
}