using System;
using System.Text;

namespace ProcMem.Memory
{
    public interface IMemory
    {
        IntPtr Handle { get; }
        byte[] Read(IntPtr address, int length);
        T Read<T>(IntPtr address);
        T[] Read<T>(IntPtr address, int length);
        string Read(IntPtr address, Encoding encoding, int maxLength);
        string Read(IntPtr address, Encoding encoding);
        int Write(IntPtr address, byte[] data);
        void Write<T>(IntPtr address, T value);
        void Write<T>(IntPtr address, T[] values);
        void Write(IntPtr address, string text, Encoding encoding);
    }
}