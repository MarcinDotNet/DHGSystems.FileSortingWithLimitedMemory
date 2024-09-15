
using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Configuration;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort;
using System.Diagnostics;
using DHGSystems.FileSortingWithLimitedMemory.Common.Helpers;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.Controllers
{
    public class FileSortingController
    {
        private readonly FileSortingAppConfiguration Configuration;
        private readonly IDhgSystemsLogger Logger;
        private const string ClassName = "FCSimple";
        
        public FileSortingController(FileSortingAppConfiguration configuration,
            IDhgSystemsLogger logger)
        {
            this.Configuration = configuration;
            this.Logger = logger;
        }

        public void SortFile(string inputFileFullFileName, string outputFileFullFileName)
        {
            Logger.Info(ClassName,$"Start sorting file {inputFileFullFileName}, file will be saved as {outputFileFullFileName}");
            if (inputFileFullFileName == null) { throw new ArgumentNullException(nameof(inputFileFullFileName)); }
            if (outputFileFullFileName == null) { throw new ArgumentNullException(nameof(outputFileFullFileName)); }
            if (!File.Exists(inputFileFullFileName)) { throw new ArgumentException("Input file does not exist"); }
            if (!Directory.Exists(Configuration.TempFolderPath))
            {
                Directory.CreateDirectory(Configuration.TempFolderPath);
            }
            
            decimal fileSizesInMB = new FileInfo(inputFileFullFileName).Length / 1024 /1024;
           
            Logger.Info(ClassName, $"File size {fileSizesInMB} MB.");

            var totalWatch = Stopwatch.StartNew();
            Logger.Info(ClassName, $"Start dividing file into sorted files. Temp folder: {Configuration.TempFolderPath}. Max lines in one file: {Configuration.MaxLinesBeforeSort}");
            var oneThreadFileDivider = new OneThreadFileDivider(Configuration.TempFolderPath, Configuration.SortedFilePrefix, Logger);
            var files = oneThreadFileDivider.DivideFileWithSort(inputFileFullFileName, Configuration.MaxLinesBeforeSort).ToArray();

            var mergeWatch = Stopwatch.StartNew();
            Logger.Info(ClassName, $"Dividing file into sorted files finished. Time {totalWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");
             fileSizesInMB = new FileInfo(files[0]).Length / 1024 / 1024;

            Logger.Info(ClassName, $"Total files created: {files.Length}.  First file size {fileSizesInMB} MB.");
            var fileMerger = new SimpleFileMergerWithSorting();
            fileMerger.MergeFilesWithSort(files,outputFileFullFileName);

            Logger.Info(ClassName, $"Merging files finished. Time {mergeWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");

            Logger.Info(ClassName, $"Sorting file {inputFileFullFileName} finished. Time {totalWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");
        }
    }
}
