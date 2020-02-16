using System;
using System.Runtime.InteropServices;
using System.Text;
using ProcMem.Native;

namespace ProcMem.Marshaling
{
    public static unsafe class MarshalType<T>
    {
        public static readonly Type RealType;
        public static readonly int Size;
        public static readonly TypeCode TypeCode;
        public static readonly bool CanBeStoredInRegisters;
        public static readonly bool IsIntPtr;

        static MarshalType()
        {
            RealType = typeof(T);
            TypeCode = Type.GetTypeCode(RealType);
            IsIntPtr = RealType == typeof(IntPtr);

            if (TypeCode == TypeCode.Boolean)
            {
                Size = 1;
            }
            else if (RealType.IsEnum)
            {
                RealType = RealType.GetEnumUnderlyingType();
                TypeCode = Type.GetTypeCode(RealType);
                Size = Marshal.SizeOf(RealType);
            }
            else
            {
                Size = Marshal.SizeOf(RealType);
            }

            CanBeStoredInRegisters = IsIntPtr ||
                                     TypeCode == TypeCode.Boolean || TypeCode == TypeCode.Char ||
                                     TypeCode == TypeCode.Byte || TypeCode == TypeCode.SByte ||
                                     TypeCode == TypeCode.Int16 || TypeCode == TypeCode.UInt16 ||
                                     TypeCode == TypeCode.Int32 || TypeCode == TypeCode.UInt32 ||
                                     TypeCode == TypeCode.Int64 || TypeCode == TypeCode.UInt64 ||
                                     TypeCode == TypeCode.Single || TypeCode == TypeCode.Double;
        }

        public static byte[] ObjectToByteArray(T obj)
        {
            switch (TypeCode)
            {
                case TypeCode.Object:
                    if (IsIntPtr)
                        switch (Size)
                        {
                            case 4: return BitConverter.GetBytes(((IntPtr)(object)obj).ToInt32());
                            case 8: return BitConverter.GetBytes(((IntPtr)(object)obj).ToInt64());
                        }
                    break;
                case TypeCode.Boolean: return BitConverter.GetBytes((bool)(object)obj);
                case TypeCode.Char: return Encoding.UTF8.GetBytes(new[] { (char)(object)obj });
                case TypeCode.Int16: return BitConverter.GetBytes((short)(object)obj);
                case TypeCode.UInt16: return BitConverter.GetBytes((ushort)(object)obj);
                case TypeCode.Int32: return BitConverter.GetBytes((int)(object)obj);
                case TypeCode.UInt32: return BitConverter.GetBytes((uint)(object)obj);
                case TypeCode.Int64: return BitConverter.GetBytes((long)(object)obj);
                case TypeCode.UInt64: return BitConverter.GetBytes((ulong)(object)obj);
                case TypeCode.Single: return BitConverter.GetBytes((float)(object)obj);
                case TypeCode.Double: return BitConverter.GetBytes((double)(object)obj);
                case TypeCode.String: throw new InvalidCastException("This method doesn't support string conversion.");
            }

            var data = new byte[Size];

            fixed (byte* pData = data)
            {
                Marshal.StructureToPtr(obj, (IntPtr)pData, false);
            }

            return data;
        }

        public static T ByteArrayToObject(byte[] data)
        {
            switch (TypeCode)
            {
                case TypeCode.Object:
                    if (IsIntPtr)
                        switch (data.Length)
                        {
                            case 1:
                                return (T)(object)new IntPtr(BitConverter.ToInt32(new byte[] { data[0], 0x0, 0x0, 0x0 }, 0));
                            case 2:
                                return (T)(object)new IntPtr(BitConverter.ToInt32(new byte[] { data[0], data[1], 0x0, 0x0 }, 0));
                            case 4: return (T)(object)new IntPtr(BitConverter.ToInt32(data, 0));
                            case 8: return (T)(object)new IntPtr(BitConverter.ToInt64(data, 0));
                        }
                    break;
                case TypeCode.Boolean: return (T)(object)BitConverter.ToBoolean(data, 0);
                case TypeCode.Byte: return (T)(object)data[0];
                case TypeCode.Char: return (T)(object)Encoding.UTF8.GetChars(data)[0]; //BitConverter.ToChar(byteArray, 0);               
                case TypeCode.Int16: return (T)(object)BitConverter.ToInt16(data, 0);
                case TypeCode.UInt16: return (T)(object)BitConverter.ToUInt16(data, 0);
                case TypeCode.Int32: return (T)(object)BitConverter.ToInt32(data, 0);
                case TypeCode.UInt32: return (T)(object)BitConverter.ToUInt32(data, 0);
                case TypeCode.Int64: return (T)(object)BitConverter.ToInt64(data, 0);
                case TypeCode.UInt64: return (T)(object)BitConverter.ToUInt64(data, 0);
                case TypeCode.Single: return (T)(object)BitConverter.ToSingle(data, 0);
                case TypeCode.Double: return (T)(object)BitConverter.ToDouble(data, 0);
                case TypeCode.String: throw new InvalidCastException("This method doesn't support string conversion.");
            }

            T obj;
            var ptrAllocated = Marshal.AllocHGlobal(Size);

            fixed (byte* pData = data)
            {
                Msvcrt.memcpy((void*)ptrAllocated, pData, Size);
                obj = (T)Marshal.PtrToStructure(ptrAllocated, typeof(T));
            }

            Marshal.FreeHGlobal(ptrAllocated);

            return obj;
        }
    }
}