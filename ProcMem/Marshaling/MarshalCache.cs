using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ProcMem.Marshaling
{
    public static class MarshalCache<T>
    {
        public unsafe delegate void* GetUnsafePtrDelegate(ref T value);
        public static readonly GetUnsafePtrDelegate GetUnsafePtr;
        public static int Size;
        public static Type RealType;
        public static TypeCode TypeCode;
        public static bool TypeRequiresMarshal;

        static MarshalCache()
        {
            var type = typeof(T);
            TypeCode = Type.GetTypeCode(type);

            if (type == typeof(bool))
            {
                RealType = type;
                Size = 1;
            }
            else if (type.IsEnum)
            {
                RealType = type.GetEnumUnderlyingType();
                Size = GetSizeOf(RealType);
                TypeCode = Type.GetTypeCode(RealType);
            }
            else
            {
                RealType = type;
                Size = GetSizeOf(type);
            }

            TypeRequiresMarshal = RequiresMarshal(RealType);
            var method = new DynamicMethod($"GetPinnedPtr<{typeof(T).FullName.Replace(".", "<>")}>",
                typeof(void*), new[] { typeof(T).MakeByRefType() }, typeof(MarshalCache<>).Module);

            var generator = method.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Conv_U);
            generator.Emit(OpCodes.Ret);

            GetUnsafePtr = (GetUnsafePtrDelegate)method.CreateDelegate(typeof(GetUnsafePtrDelegate));
        }

        private static int GetSizeOf(Type type)
        {
            try
            {
                return Marshal.SizeOf(type);
            }
            catch
            {
                var totalSize = 0;

                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var attr = field.GetCustomAttributes(typeof(FixedBufferAttribute), false);

                    if (attr.Length > 0)
                    {
                        var fba = attr[0] as FixedBufferAttribute;
                        totalSize += GetSizeOf(fba.ElementType) * fba.Length;
                    }

                    totalSize += GetSizeOf(field.FieldType);
                }
                return totalSize;
            }
        }

        private static bool RequiresMarshal(Type type)
        {
            foreach (var fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var requires = fieldInfo.GetCustomAttributes(typeof(MarshalAsAttribute), true).Any();

                if (requires) return true;
                if (type == typeof(IntPtr) || type == typeof(string)) continue;
                if (Type.GetTypeCode(type) == TypeCode.Object) requires |= RequiresMarshal(fieldInfo.FieldType);
                if (requires) return true;
            }
            return false;
        }
    }
}