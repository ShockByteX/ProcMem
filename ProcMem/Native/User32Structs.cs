using System;
using System.Runtime.InteropServices;

namespace ProcMem.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Input
    {
        public InputType Type;
        public InputUnion Union;
        public static int Size => Marshal.SizeOf(typeof(Input));
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)]
        public MouseInput MouseInput;
        [FieldOffset(0)]
        public KeyboardInput KeyboardInput;
        [FieldOffset(0)]
        public HardwareInput HardwareInput;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInput
    {
        public int DX, DY, MouseData;
        public MouseEventF DwFlags;
        public uint Time;
        public UIntPtr DwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInput
    {
        public VirtualKeyShort VirtualKey;
        public short ScanCode;
        public KeyEventF DwFlags;
        public int Time;
        public UIntPtr DwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardHookInput
    {
        public int VirtualKey;
        public int ScanCode;
        public int DwFlags;
        public int Time;
        public UIntPtr DwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInput
    {
        public int UMsg;
        public short WParamL, WParamH;
    }
}