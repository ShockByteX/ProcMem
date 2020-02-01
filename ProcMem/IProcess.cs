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
        IReadOnlyCollection<MemoryRegion> GetMemoryRegions();
    }
}