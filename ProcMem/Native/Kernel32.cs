using System;
using System.Runtime.InteropServices;

namespace ProcMem.Native
{
    public unsafe class Kernel32
    {
        public const string LibraryName = "kernel32.dll";

        [DllImport(LibraryName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport(LibraryName, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport(LibraryName, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryProtectionFlags flNewProtect, out MemoryProtectionFlags lpflOldProtect);

        [DllImport(LibraryName, EntryPoint = "VirtualQueryEx")]
        public static extern int VirtualQueryEx32(IntPtr hProcess, IntPtr lpAddress, out MemoryBasicInformation32 lpBuffer, int dwLength);

        [DllImport(LibraryName, EntryPoint = "VirtualQueryEx")]
        public static extern int VirtualQueryEx64(IntPtr hProcess, IntPtr lpAddress, out MemoryBasicInformation64 lpBuffer, int dwLength);

        [DllImport(LibraryName, EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void MoveMemory(void* dest, void* src, int size);

        [DllImport(LibraryName, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport(LibraryName, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport(LibraryName)]
        public static extern void GetSystemInfo(out SystemInfo lpSystemInfo);

        public static int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MemoryBasicInformation lpBuffer)
        {
            int result;

            switch (IntPtr.Size)
            {
                case 4:
                    result = VirtualQueryEx32(hProcess, lpAddress, out var memoryInfo32, MemoryBasicInformation32.StructSize);
                    lpBuffer = memoryInfo32;
                    break;
                case 8:
                    result = VirtualQueryEx64(hProcess, lpAddress, out var memoryInfo64, MemoryBasicInformation64.StructSize);
                    lpBuffer = memoryInfo64;
                    break;
                default: throw new NotSupportedException();
            }

            return result;
        }
    }
}