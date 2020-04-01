using System;
using System.Collections.Generic;
using System.Linq;
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
            var addresses = SignatureScanner.Scan(Data, pattern, firstOnly).Select(x =>
            {
                var address = IntPtr.Add(Address, x + offset);

                if (relative)
                {
                    address = ExProcess.Memory.Read<IntPtr>(address);
                }

                return IntPtr.Add(address, extra);
            });

            return addresses;
        }

        public bool Equals(MemorySnapshot other)
        {
            if (other is null) return false;
            return Address.Equals(other.Address) && Data.Length.Equals(other.Data.Length);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case MemorySnapshot snapshot:
                    return Equals(snapshot);
                default:
                    return false;
            }
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
                    if (!memoryRegion.Readable) continue;

                    var buffer = memoryRegion.Read(0, (int)memoryRegion.Information.RegionSize);
                    var offset = memoryRegion.Address.ToInt64() - address.ToInt64();

                    fixed (byte* ptrBuffer = buffer)
                    {
                        Msvcrt.memcpy(IntPtr.Add(dataAddress, (int)offset), (IntPtr)ptrBuffer, buffer.Length);
                    }
                }
            }

            return new MemorySnapshot(exProcess, address, data);
        }
    }
}