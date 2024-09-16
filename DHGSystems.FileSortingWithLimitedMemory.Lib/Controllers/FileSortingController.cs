
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
        private readonly FileSortingAppConfiguration _configuration;
        private readonly IDhgSystemsLogger _logger;
        private const string ClassName = "FCSimple";
        private readonly IFileDividerWithSort _fileSorterDividerWithSort;
        private readonly IFileMergerWithSorting _fileMergerWithSorting;
        
        public FileSortingController(FileSortingAppConfiguration configuration,
            IDhgSystemsLogger logger, IFileDividerWithSort fileSorterDividerWithSort, IFileMergerWithSorting fileMergerWithSorting)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._fileSorterDividerWithSort = fileSorterDividerWithSort;
            this._fileMergerWithSorting = fileMergerWithSorting;

        }

        public void SortFile(string inputFileFullFileName, string outputFileFullFileName)
        {
            _logger.Info(ClassName,$"Start sorting file {inputFileFullFileName}, file will be saved as {outputFileFullFileName}");
            if (inputFileFullFileName == null) { throw new ArgumentNullException(nameof(inputFileFullFileName)); }
            if (outputFileFullFileName == null) { throw new ArgumentNullException(nameof(outputFileFullFileName)); }
            if (!File.Exists(inputFileFullFileName)) { throw new ArgumentException("Input file does not exist"); }
            if (!Directory.Exists(_configuration.TempFolderPath))
            {
                Directory.CreateDirectory(_configuration.TempFolderPath);
            }
            
            decimal fileSizesInMB = new FileInfo(inputFileFullFileName).Length / 1024 /1024;
           
            _logger.Info(ClassName, $"File size {fileSizesInMB} MB.");

            var totalWatch = Stopwatch.StartNew();
            _logger.Info(ClassName, $"Start dividing file into sorted files. Temp folder: {_configuration.TempFolderPath}. Max lines in one file: {_configuration.MaxLinesBeforeSort}");
             var files = this._fileSorterDividerWithSort.DivideFileWithSort(inputFileFullFileName, _configuration.MaxLinesBeforeSort).ToArray();

            var mergeWatch = Stopwatch.StartNew();
            _logger.Info(ClassName, $"Dividing file into sorted files finished. Time {totalWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");
             fileSizesInMB = new FileInfo(files[0]).Length / 1024 / 1024;

            _logger.Info(ClassName, $"Total files created: {files.Length}.  First file size {fileSizesInMB} MB.");
         
            _fileMergerWithSorting.MergeFilesWithSort(files,outputFileFullFileName);

            _logger.Info(ClassName, $"Merging files finished. Time {mergeWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");

            _logger.Info(ClassName, $"Sorting file {inputFileFullFileName} finished. Time {totalWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");
        }
    }
}
