﻿using System;

namespace ProcMem.Native
{
    [Flags]
    public enum ProcessAccessFlags
    {
        AllAccess = 0x001F0FFF,
        CreateProcess = 0x0080,
        CreateThread = 0x0002,
        DuplicateHandle = 0x0040,
        QueryInformation = 0x0400,
        QueryLimitedInformation = 0x1000,
        SetInformation = 0x0200,
        SetQuota = 0x0100,
        SuspendResume = 0x0800,
        Terminate = 0x0001,
        VmOperation = 0x0008,
        VmRead = 0x0010,
        VmWrite = 0x0020,
        Synchronize = 0x00100000
    }

    [Flags]
    public enum MemoryProtectionFlags : uint
    {
        ZeroAccess = 0x0,
        NoAccess = 0x1,
        ReadOnly = 0x2,
        ReadWrite = 0x4,
        WriteCopy = 0x8,
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        Guard = 0x100,
        NoCache = 0x200,
        WriteCombine = 0x400
    }

    [Flags]
    public enum MemoryStateFlags : uint
    {
        Commit = 0x1000,
        Free = 0x10000,
        Reserve = 0x2000
    }

    [Flags]
    public enum MemoryTypeFlags : uint
    {
        None = 0x0,
        Image = 0x1000000,
        Mapped = 0x40000,
        Private = 0x20000
    }
}