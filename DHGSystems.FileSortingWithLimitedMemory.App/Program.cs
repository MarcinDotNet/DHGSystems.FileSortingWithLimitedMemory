using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Configuration;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Controllers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort;
using DHGSystems.FileSortingWithLimitedMemory.Lib.TestDataGenerator;
using System.Diagnostics;
using DHGSystems.FileSortingWithLimitedMemory.Common.Helpers;

namespace DHGSystems.FileSortingWithLimitedMemory.App
{
    internal class Program
    {
        private static string _tempPath = "SortingTempFolder";
        private static string _sortedFileName = @"SortingTempFolder\SortedFile.txt";
        private static string _fileToSortName = @"SortingTempFolder\FileToSort.txt";
        private static IDhgSystemsLogger _logger = new DhgSystemsNLogLogger();

        private static void Main(string[] args)
        {
            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }

            string fileToSortName = @"SortingTempFolder\FileToSort.txt";
            if (args.Length < 1)
            {
                _logger.Info("Usage: FileSortingWithLimitedMemory <input file>. Output file is always sorted");
                _logger.Info("You can also set as random file parameter: Random1GB, Random10GB, Random100GB");
                _logger.Info("Now we set parameter to Random1GB");
                args = new string[] { "Random1GB" };
            }
            
            var watcher = Stopwatch.StartNew();
            if (args[0].ToLower().Contains("random"))
            {
                int maxRowSize = 1024;
                int diffValues = 500000;
                int rowsFactorFor1GB = 15500003 / 8;

                RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(maxRowSize, diffValues, true);
                if (args[0].ToLower() == "random1gb")
                {
                    _logger.Error($"Start generating 1 GB file with {rowsFactorFor1GB} rows. Max rows size {maxRowSize}. Different values {diffValues} ");
                    randomStringFileGenerator.GenerateTestFile(rowsFactorFor1GB, _fileToSortName);
                }
                if (args[0].ToLower() == "random10gb")
                {
                    _logger.Error($"Start generating 10 GB file with {rowsFactorFor1GB *10} rows. Max rows size {maxRowSize}. Different values {diffValues} ");
                    randomStringFileGenerator.GenerateTestFile((rowsFactorFor1GB) * 10, _fileToSortName);
                }

                if (args[0].ToLower() == "random100gb")
                {
                    _logger.Error($"Start generating 100 GB file with {rowsFactorFor1GB * 100} rows. Max rows size {maxRowSize}. Different values {diffValues} ");

                    randomStringFileGenerator.GenerateTestFile((rowsFactorFor1GB) * 100, _fileToSortName);
                }
                _logger.Info($"File  {fileToSortName} generated in {watcher.ElapsedMilliseconds} ms");
            }
            else
            {
                fileToSortName = args[0];
            }

            if (!File.Exists(fileToSortName))
            {
                _logger.Error("File does not exist.Stopping");
                return;
            }

            FileSortingAppConfiguration fileSortingAppConfiguration = new FileSortingAppConfiguration()
            {
                MaxLinesBeforeSort = 1500000, // 1500000 * 1024B  *2  = 3 GB * 4 workers = 8 GB of memory * 3.5 OrderBy mesh at one time = 28 GB of memory
                SortedFilePrefix = "sorted_file_",
                TempFolderPath = _tempPath,
                StartMergeFileCount = 5
            };

            if (FileHelper.GetFileSizeInMB(fileToSortName) > 12000)
            {
                _logger.Error("File bigger then 12000 MB StartMergeFileCount set to 7 ");
                fileSortingAppConfiguration.StartMergeFileCount = 7;// more work on CPU
            }
            _logger.Info($"Configuration set to  MaxLinesBeforeSort {fileSortingAppConfiguration.MaxLinesBeforeSort}, StartMergeFileCount {fileSortingAppConfiguration.StartMergeFileCount}, TempFolderPath {fileSortingAppConfiguration.TempFolderPath}");
            var fileDivider = new MultiWorkersFileDivider(fileSortingAppConfiguration.TempFolderPath, fileSortingAppConfiguration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(fileSortingAppConfiguration, new DhgSystemsNLogLogger(), fileDivider, new FileMergerWithSortingCumulated(new DhgSystemsNLogLogger()));
            controller.SortFile(fileToSortName, _sortedFileName);
            _logger.Info("Completed");
        }
    }
}