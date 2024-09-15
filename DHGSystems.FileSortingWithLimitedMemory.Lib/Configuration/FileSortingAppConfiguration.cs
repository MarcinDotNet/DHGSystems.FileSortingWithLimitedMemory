namespace DHGSystems.FileSortingWithLimitedMemory.Lib.Configuration
{
    public class FileSortingAppConfiguration
    {
        public string SortedFilePrefix { get; set; }
        public string TempFolderPath { get; set; }
        public long MaxLinesBeforeSort { get; set; }
    }
}
