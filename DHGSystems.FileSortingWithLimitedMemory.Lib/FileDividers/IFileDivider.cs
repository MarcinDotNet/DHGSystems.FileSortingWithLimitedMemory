using System.Collections.Concurrent;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers
{
    public interface IFileDividerWithSort
    {
       void DivideFileWithSort(string fileToDived, long maxLinesBeforeSort, ConcurrentQueue<string> filesProcessed);
    }
}
