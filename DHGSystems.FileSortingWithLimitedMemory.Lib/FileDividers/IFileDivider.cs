namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers
{
    public interface IFileDividerWithSort
    {
        IEnumerable<string> DivideFileWithSort(string fileToDived, long maxLinesBeforeSort);
    }
}
