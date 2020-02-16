using System;
using System.Collections.Generic;
using ProcMem.Native;
using ProcMem.Utilities;

namespace ProcMem.Memory
{
    public class MemorySnapshot : IEquatable<MemorySnapshot>
    {
        public readonly IProcess ExProcess;
        public readonly IntPtr Address;
        public readonly byte[] Data;

        public MemorySnapshot(IProcess exProcess, IntPtr address, byte[] data)
        {
            ExProcess = exProcess;
            Address = address;
            Data = data;
        }

        public IEnumerable<IntPtr> ScanSignature(string pattern, int offset, int extra, bool relative, bool firstOnly = true)
        {
            var foundOffsets = SignatureScanner.Scan(Data, pattern, firstOnly);
            var addresses = new List<IntPtr>();

            foreach (var foundOffset in foundOffsets)
            {
                var address = IntPtr.Add(Address, foundOffset + offset);
                address = IntPtr.Add(ExProcess.Memory.Read<IntPtr>(address), extra);
                addresses.Add(relative ? address : address);
            }

            return addresses;
        }

        public IEnumerable<IntPtr> ScanSignature(byte[] signature, byte unknownByte, int extra, int offset, bool relative)
        {
            var foundOffsets = SignatureScanner.Scan(Data, signature, unknownByte);
            var addresses = new List<IntPtr>();

            foreach (var foundOffset in foundOffsets)
            {
                var address = IntPtr.Add(Address, foundOffset + extra);
                address = relative ? IntPtr.Add(ExProcess.Memory.Read<IntPtr>(address), offset) : IntPtr.Add(address, offset);
                addresses.Add(address);
            }

            return addresses;
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

        public static unsafe MemorySnapshot Create(IProcess exProcess, IntPtr address, int size)
        {
            var memoryRegions = exProcess.GetMemoryRegions(address, size);
            var data = new byte[size];

            fixed (byte* ptrData = data)
            {
                var dataAddress = (IntPtr)ptrData;

                foreach (var memoryRegion in memoryRegions)
                {
                    if (!memoryRegion.IsReadable) continue;

                    var buffer = memoryRegion.Read(0, memoryRegion.Information.RegionSize);
                    var offset = memoryRegion.Address.ToInt64() - address.ToInt64();

                    fixed (byte* ptrBuffer = buffer)
                    {
                        Msvcrt.memcpy(IntPtr.Add(dataAddress, (int)offset), (IntPtr)ptrBuffer, buffer.Length);
                    }
                }
            }

            return new MemorySnapshot(exProcess, address, data);
        }

        public static MemorySnapshot Create(IProcess exProcess, IntPtr address, int size, bool forced)
        {
            var data = exProcess[address].Read(0, size);
            return new MemorySnapshot(exProcess, address, data);
        }
    }
}