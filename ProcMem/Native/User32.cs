using System;
using System.Runtime.InteropServices;

namespace ProcMem.Native
{
    public delegate IntPtr KeyboardHookProc(int nCode, WindowMessages wParam, KeyboardHookInput lParam);
    public static class User32
    {
        public const string LibraryName = "user32.dll";
        [DllImport(LibraryName)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, KeyboardHookProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport(LibraryName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport(LibraryName)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport(LibraryName)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, WindowMessages wParam, ref KeyboardHookInput lParam);
        [DllImport(LibraryName)]
        public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] Input[] pInputs, int cbSize);
        [DllImport(LibraryName)]
        public static extern uint MapVirtualKey(uint uCode, VirtualKeyMapType uMapType);
        [DllImport(LibraryName)]
        public static extern uint MapVirtualKeyEx(uint uCode, VirtualKeyMapType uMapType, IntPtr dwhkl);
    }
}