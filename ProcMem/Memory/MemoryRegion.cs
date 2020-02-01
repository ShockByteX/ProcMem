using System;
using ProcMem.Native;
using ProcMem.Utilities;

namespace ProcMem.Memory
{
    public unsafe class MemoryRegion : MemoryPointer, IEquatable<MemoryRegion>
    {
        public MemoryRegion(IMemory memory, IntPtr address) : base(memory, address) { }

        public override bool IsValid => base.IsValid && Information.State != MemoryStateFlags.Free;
        public MemoryBasicInformation Information => MemoryHelper.Query(Memory.Handle, Address);

        public IntPtr Scan(string strPattern, int extra, int offset, bool relative)
        {
            var buffer = Read(0, Information.RegionSize);
            var pattern = ParseHelper.BytesFromPattern(strPattern, out byte dByte);
            var pLength = pattern.Length;
            var endPoint = buffer.Length - pLength + 1;

            fixed (byte* pPattern = pattern, pBuffer = buffer)
            {
                for (var i = 0; i < endPoint; i++)
                {
                    if (pBuffer[i] == pPattern[0])
                    {
                        int j;
                        for (j = 0; j < pLength; j++)
                        {
                            if (pPattern[j] != dByte && pPattern[j] != pBuffer[j + i]) break;
                        }
                        if (j != pLength)
                        {
                            i += j;
                            continue;
                        }
                        var address = IntPtr.Add(Address, i + extra);
                        return relative ? IntPtr.Add((IntPtr)Memory.Read<int>(address), offset) : IntPtr.Add(address, offset);
                    }
                }
            }
            return IntPtr.Zero;
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