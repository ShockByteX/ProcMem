﻿using System;
using System.Runtime.InteropServices;
using ProcMem.Marshaling;
using ProcMem.Native;

namespace ProcMem.Extensions
{
    public static unsafe class UnsafeMemoryExtensions
    {
        public static IntPtr GetVTableIntPtr(this IntPtr address, int functionIndex) => (address.Read<IntPtr>() + functionIndex * IntPtr.Size).Read<IntPtr>();
        public static IntPtr ToFunctionPtr(this Delegate d) => Marshal.GetFunctionPointerForDelegate(d);
        public static TDelegate ToDelegate<TDelegate>(this IntPtr address) where TDelegate : class => Marshal.GetDelegateForFunctionPointer<TDelegate>(address);

        public static T Read<T>(this IntPtr address)
        {
            try
            {
                if (address == IntPtr.Zero) throw new InvalidOperationException("Cannot retrieve a value at address 0");

                object ptrToStructure;

                switch (MarshalCache<T>.TypeCode)
                {
                    case TypeCode.Object:
                        if (MarshalCache<T>.RealType == typeof(IntPtr))
                        {
                            return (T)(object)*(IntPtr*)address;
                        }

                        if (!MarshalCache<T>.TypeRequiresMarshal)
                        {
                            T obj = default;
                            var ptr = MarshalCache<T>.GetUnsafePtr(ref obj);
                            Kernel32.MoveMemory(ptr, (void*)address, MarshalCache<T>.Size);
                            return obj;
                        }

                        ptrToStructure = Marshal.PtrToStructure(address, typeof(T));
                        break;
                    case TypeCode.Boolean:
                        ptrToStructure = *(byte*)address != 0;
                        break;
                    case TypeCode.Char:
                        ptrToStructure = *(char*)address;
                        break;
                    case TypeCode.SByte:
                        ptrToStructure = *(sbyte*)address;
                        break;
                    case TypeCode.Byte:
                        ptrToStructure = *(byte*)address;
                        break;
                    case TypeCode.Int16:
                        ptrToStructure = *(short*)address;
                        break;
                    case TypeCode.UInt16:
                        ptrToStructure = *(ushort*)address;
                        break;
                    case TypeCode.Int32:
                        ptrToStructure = *(int*)address;
                        break;
                    case TypeCode.UInt32:
                        ptrToStructure = *(uint*)address;
                        break;
                    case TypeCode.Int64:
                        ptrToStructure = *(long*)address;
                        break;
                    case TypeCode.UInt64:
                        ptrToStructure = *(ulong*)address;
                        break;
                    case TypeCode.Single:
                        ptrToStructure = *(float*)address;
                        break;
                    case TypeCode.Double:
                        ptrToStructure = *(double*)address;
                        break;
                    default: throw new NotSupportedException();
                }

                return (T)ptrToStructure;
            }
            catch (AccessViolationException)
            {
                return default;
            }
        }
    }
}