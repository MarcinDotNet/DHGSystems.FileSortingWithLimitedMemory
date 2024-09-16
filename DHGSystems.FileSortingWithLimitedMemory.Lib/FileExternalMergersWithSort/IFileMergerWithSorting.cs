namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort
{
    public interface IFileMergerWithSorting
    {
        void MergeFilesWithSort(string[] filesToMerge, string outputFilePath);
    }
}