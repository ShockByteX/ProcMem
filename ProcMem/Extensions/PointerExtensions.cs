using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ProcMem.Extensions
{
    public static class PointerExtensions
    {
        public static void Validate(this IntPtr handle, bool lastWin32Error = false)
        {
            if (handle == IntPtr.Zero)
            {
                if (lastWin32Error) throw new Win32Exception(Marshal.GetLastWin32Error());
                throw new ArgumentException("The handle is not valid", nameof(handle));
            }
        }
    }
}