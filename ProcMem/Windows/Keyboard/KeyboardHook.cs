using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ProcMem.Extensions;
using ProcMem.Native;

namespace ProcMem.Windows.Keyboard
{
    public class KeyboardHook
    {
        private IntPtr _hook;
        private readonly KeyboardHookProc _hookProc;

        public KeyboardHook(string name)
        {
            Name = name;
            _hookProc = HookCallback;
        }

        public string Name { get; }
        public bool IsEnabled { get; private set; }
        public bool IsDisposed { get; }

        public void Enable()
        {
            _hook = User32.SetWindowsHookEx(HookType.KeyboardLL, _hookProc, Kernel32.GetModuleHandle(Process.GetCurrentProcess().MainModule?.ModuleName), 0);
            _hook.Validate();
        }
        public void Disable()
        {
            User32.UnhookWindowsHookEx(_hook);
            IsEnabled = false;
        }

        private IntPtr HookCallback(int code, WindowMessages wParam, KeyboardHookInput lParam)
        {
            IntPtr result;

            try
            {
                if (code >= 0)
                {
                    var key = (VirtualKey)Marshal.ReadInt32(new IntPtr(lParam.VirtualKey));

                    if (wParam == WindowMessages.KeyDown || wParam == WindowMessages.SysKeyDown)
                    {
                        KeyDown?.Invoke(this, key);
                    }

                    if (wParam == WindowMessages.KeyUp || wParam == WindowMessages.SysKeyUp)
                    {
                        KeyUp?.Invoke(this, key);
                    }
                }
            }
            finally
            {
                result = User32.CallNextHookEx(_hook, code, wParam, ref lParam);
            }

            return result;
        }

        public event EventHandler<VirtualKey> KeyDown;
        public event EventHandler<VirtualKey> KeyUp;
    }
}