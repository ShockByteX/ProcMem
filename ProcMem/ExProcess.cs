using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ProcMem.Extensions;
using ProcMem.Memory;
using ProcMem.Native;
using ProcMem.Utilities;
using ProcMem.Windows.Keyboard;
using ProcMem.Windows.Mouse;

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
                default: throw new NotSupportedException();
            }

            Process.EnableRaisingEvents = true;
            Process.Exited += (s, e) => ProcessExited?.Invoke(this);
        }
        ~ExProcess() => Dispose();

        public static IKeyboard GlobalKeyboard => MessageKeyboard.Instance;
        public static IMouse GlobalMouse => MessageMouse.Instance;

        public Process Process { get; }
        public IntPtr Handle { get; }
        public IMemory Memory { get; }
        public IPointer this[IntPtr address] => new MemoryPointer(Memory, address);

        public IEnumerable<MemoryRegion> GetMemoryRegions()
        {
            var regions = new List<MemoryRegion>();

            Kernel32.GetSystemInfo(out var systemInfo);

            var maxAddress = (long)systemInfo.MaximumApplicationAddress;
            var currentAddress = IntPtr.Zero;

            while (MemoryHelper.Query(Handle, currentAddress, out var info) > 0 && currentAddress.ToInt64() < maxAddress)
            {
                regions.Add(new MemoryRegion(Memory, currentAddress));
                currentAddress = info.BaseAddress.Add(info.RegionSize);
            }

            return regions;
        }

        public IEnumerable<MemoryRegion> GetMemoryRegions(IntPtr address, int size)
        {
            var regions = new List<MemoryRegion>();
            var currentAddress = address;
            var endAddress = IntPtr.Add(address, size);

            while (currentAddress.ToInt64() < endAddress.ToInt64())
            {
                MemoryHelper.Query(Handle, currentAddress, out var info);
                regions.Add(new MemoryRegion(Memory, currentAddress));
                currentAddress = info.BaseAddress + (int)info.RegionSize;
            }

            return regions;
        }

        public IEnumerable<IntPtr> ScanSignature(string pattern, int extra, int offset, bool relative, bool firstOnly = true)
        {
            var regions = GetMemoryRegions().Where(x => x.Readable);
            var result = new List<IntPtr>();

            Parallel.ForEach(regions, (region) =>
            {
                var data = Memory.Read(region.Address, (int)region.Information.RegionSize);

                result.AddRange(SignatureScanner.Scan(data, pattern, firstOnly).Select(x =>
                 {
                     var address = IntPtr.Add(region.Address, x + offset);

                     if (relative)
                     {
                         address = Memory.Read<IntPtr>(address);
                     }

                     return IntPtr.Add(address, extra);
                 }));

            });

            GC.Collect();

            return result;
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