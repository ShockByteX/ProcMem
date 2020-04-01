using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProcMem.Utilities;

namespace ProcMem.Memory
{
    public class MemoryPointer : IEquatable<MemoryPointer>, IPointer
    {
        public readonly IMemory Memory;

        public MemoryPointer(IMemory memory, IntPtr address)
        {
            Memory = memory;
            Address = address;
        }

        public IntPtr Address { get; }
        public virtual bool IsValid => Address != IntPtr.Zero;

        public byte[] Read(int offset, int length) => Memory.Read(IntPtr.Add(Address, offset), length);
        public T Read<T>(int offset) => Memory.Read<T>(IntPtr.Add(Address, offset));
        public T[] Read<T>(int offset, int count) => Memory.Read<T>(IntPtr.Add(Address, offset), count);
        public string Read(int offset, Encoding encoding, int maxLength) => Memory.Read(IntPtr.Add(Address, offset), encoding, maxLength);
        public string Read(int offset, Encoding encoding) => Memory.Read(IntPtr.Add(Address, offset), encoding);

        public int Write(int offset, byte[] data) => Memory.Write(IntPtr.Add(Address, offset), data);
        public void Write<T>(int offset, T value) => Memory.Write(IntPtr.Add(Address, offset), value);
        public void Write<T>(int offset, T[] values) => Memory.Write(IntPtr.Add(Address, offset), values);
        public void Write(int offset, string text, Encoding encoding) => Memory.Write(IntPtr.Add(Address, offset), text, encoding);

        public IEnumerable<IntPtr> ScanSignature(string pattern, int size, int offset, int extra, bool relative, bool firstOnly = true)
        {
            var addresses = SignatureScanner.Scan(Read(0, size), pattern).Select(x =>
            {
                var address = IntPtr.Add(Address, x + offset);

                if (relative)
                {
                    address = Memory.Read<IntPtr>(address);
                }

                return IntPtr.Add(address, extra);
            });

            return addresses;
        }

        public bool Equals(MemoryPointer other) => other != null && Address.Equals(other.Address);

        public override int GetHashCode() => Address.GetHashCode();
        public override string ToString() => $"Address: 0x{Address.ToInt64():X}";
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null: return false;
                case MemoryPointer pointer: return Equals(pointer);
                default: return false;
            }
        }

        public static bool operator ==(MemoryPointer left, MemoryPointer right) => Equals(left, right);
        public static bool operator !=(MemoryPointer left, MemoryPointer right) => !Equals(left, right);
    }
}