using System.Diagnostics;

namespace DHGSystems.FileSortingWithLimitedMemory.Common.Helpers
{
    public static class ProcessHelper
    {
        public static long GetUsedMemoryInMb()
        {
            var proc = Process.GetCurrentProcess();
            return proc.PrivateMemorySize64 / 1024 / 1024;
        }
    }
}