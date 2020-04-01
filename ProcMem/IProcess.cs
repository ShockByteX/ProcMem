using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProcMem.Memory;

namespace ProcMem
{
    public interface IProcess
    {
        Process Process { get; }
        IntPtr Handle { get; }
        IMemory Memory { get; }
        IPointer this[IntPtr address] { get; }
        IEnumerable<MemoryRegion> GetMemoryRegions();
        IEnumerable<MemoryRegion> GetMemoryRegions(IntPtr address, int size);
        IEnumerable<IntPtr> ScanSignature(string pattern, int extra, int offset, bool relative, bool firstOnly = true);
    }
}