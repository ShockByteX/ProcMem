using System;
using System.Text;
using ProcMem.Marshaling;

namespace ProcMem.Memory
{
    public abstract class ProcessMemory : IMemory
    {
        public const char NullTerminator = '\0';

        protected ProcessMemory(IntPtr handle) => Handle = handle;

        public IntPtr Handle { get; }

        public abstract byte[] Read(IntPtr address, int length);

        public abstract T Read<T>(IntPtr address);
        public T[] Read<T>(IntPtr address, int length)
        {
            var values = new T[length];

            for (var i = 0; i < length; i++)
            {
                values[i] = Read<T>(IntPtr.Add(address, i * MarshalType<T>.Size));
            }

            return values;
        }

        public string Read(IntPtr address, Encoding encoding, int maxLength)
        {
            var text = encoding.GetString(Read(address, maxLength));
            var ntIndex = text.IndexOf(NullTerminator);

            return ntIndex != -1 ? text.Remove(ntIndex) : text;
        }
        public string Read(IntPtr address, Encoding encoding)
        {
            var result = string.Empty;
            var offset = 0;
            char c;

            while ((c = Read<char>(IntPtr.Add(address, offset++))) != NullTerminator)
            {
                result += c;
            }

            return result;
        }

        public abstract int Write(IntPtr address, byte[] buffer);

        public abstract void Write<T>(IntPtr address, T value);
        public void Write<T>(IntPtr address, T[] values)
        {
            var length = values.Length;

            for (var i = 0; i < length; i++)
            {
                Write(IntPtr.Add(address, i * MarshalType<T>.Size), values[i]);
            }
        }

        public virtual void Write(IntPtr address, string text, Encoding encoding)
        {
            if (text[text.Length - 1] != NullTerminator)
            {
                text += NullTerminator;
            }

            Write(address, encoding.GetBytes(text));
        }
    }
}