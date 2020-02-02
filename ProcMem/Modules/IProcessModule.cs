using System.Diagnostics;

namespace ProcMem.Modules
{
    public interface IProcessModule
    {
        ProcessModule NativeModule { get; }
        string Name { get; }
        string Path { get; }
        int Size { get; }
        bool IsMainModule { get; }
    }
}