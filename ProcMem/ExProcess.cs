using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProcMem.Extensions;
using ProcMem.Memory;
using ProcMem.Native;
using ProcMem.Windows.Keyboard;

namespace ProcMem
{
    public class ExProcess : IProcess, IDisposable
    {
        private bool _disposed;

        public ExProcess(Process process, MemoryAccessType memoryAccessType)
        {
            Process = process;

            Handle = Kernel32.OpenProcess(ProcessAccessFlags.AllAccess, false, process.Id);
            Handle.Validate(true);

            switch (memoryAccessType)
            {
                case MemoryAccessType.Local:
                    Memory = new ProcessMemoryLocal(Handle);
                    break;
                case MemoryAccessType.Remote:
                    Memory = new ProcessMemoryRemote(Handle);
                    break;
            }

            Process.EnableRaisingEvents = true;
            Process.Exited += (s, e) => ProcessExited?.Invoke(this);
        }
        ~ExProcess() => Dispose();

        public static IKeyboard GlobalKeyboard => Keyboard.Instance;

        public Process Process { get; }
        public IntPtr Handle { get; }
        public IMemory Memory { get; }
        public IPointer this[IntPtr address] => new MemoryPointer(Memory, address);

        public IReadOnlyCollection<MemoryRegion> GetMemoryRegions()
        {
            var regions = new List<MemoryRegion>();

            Kernel32.GetSystemInfo(out var sysInfo);

            var currentAddress = IntPtr.Zero;

            while (Kernel32.VirtualQueryEx(Handle, currentAddress, out var info, MemoryBasicInformation.StructSize) > 0)
            {
                regions.Add(new MemoryRegion(Memory, currentAddress));
                currentAddress = info.BaseAddress + info.RegionSize;
            }

            return regions;
        }

        public unsafe IEnumerable<IntPtr> Scan(IntPtr address, int size, byte[] data, int extra, int offset, bool relative, int dByte = 0x0)
        {
            var addresses = new List<IntPtr>();
            var buffer = Memory.Read(address, size);

            var endPoint = buffer.Length - data.Length + 1;
            var sigByte = data[0];

            fixed (byte* ptrData = data, ptrBuffer = buffer)
            {
                for (var i = 0; i < endPoint; i++)
                {
                    if (sigByte.Equals(ptrBuffer[i]))
                    {
                        int j;

                        for (j = 0; j < data.Length; j++)
                        {
                            if (ptrData[j] != dByte && ptrData[j] != ptrBuffer[j + i]) break;
                        }

                        if (j != data.Length)
                        {
                            i += j;
                            continue;
                        }

                        var pointer = IntPtr.Add(address, i + extra);
                        addresses.Add(IntPtr.Add(relative ? Memory.Read<IntPtr>(pointer) : pointer, offset));
                    }
                }
            }

            return addresses;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Process.Dispose();
            Kernel32.CloseHandle(Handle);
        }

        public event Action<ExProcess> ProcessExited;
    }
}