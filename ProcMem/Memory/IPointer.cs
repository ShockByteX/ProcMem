using System;
using System.Collections.Generic;
using System.Text;

namespace ProcMem.Memory
{
    public interface IPointer
    {
        IntPtr Address { get; }
        bool IsValid { get; }
        byte[] Read(int offset, int length);
        T Read<T>(int offset);
        T[] Read<T>(int offset, int length);
        string Read(int offset, Encoding encoding, int maxLength);
        string Read(int offset, Encoding encoding);
        void Write(int offset, string text, Encoding encoding);
        void Write<T>(int offset, T value);
        void Write<T>(int offset, T[] values);
        int Write(int offset, byte[] data);
        IEnumerable<IntPtr> ScanSignature(string pattern, int extra, int offset, bool relative, int size);
    }
}