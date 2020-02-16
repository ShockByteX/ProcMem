using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMem.Memory
{
    public interface IMemoryScanner
    {
        IEnumerable<IntPtr> ScanSignature(string pattern, int extra, int offset, bool relative);
        IEnumerable<IntPtr> ScanSignature(byte[] data, byte[] signature, byte unknownByte, int extra, int offset, bool relative);
    }
}
