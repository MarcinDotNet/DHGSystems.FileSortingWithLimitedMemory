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
                RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(1024, 500000, true);
                if (args[0].ToLower() == "random1gb")
                {
                    randomStringFileGenerator.GenerateTestFile(16500003, _fileToSortName);
                }
                if (args[0].ToLower() == "random10gb")
                {
                    randomStringFileGenerator.GenerateTestFile(16500003 * 10, _fileToSortName);
                }

                if (args[0].ToLower() == "random100gb")
                {
                    randomStringFileGenerator.GenerateTestFile(16500003 * 100, _fileToSortName);
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
                MaxLinesBeforeSort = 1500000, // 1500000 * 1024B *2  = 3 GB * 4 workers = 8 GB of memory * 3.5 OrderBy mesh at one time = 28 GB of memory
                SortedFilePrefix = "sorted_file_",
                TempFolderPath = _tempPath,
                StartMergeFileCount = 5
            };

            if (FileHelper.GetFileSizeInMB(fileToSortName) > 12000)
            {
                fileSortingAppConfiguration.StartMergeFileCount = 7;
            }
            
            var fileDivider = new MultiWorkersFileDivider(fileSortingAppConfiguration.TempFolderPath, fileSortingAppConfiguration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(fileSortingAppConfiguration, new DhgSystemsNLogLogger(), fileDivider, new FileMergerWithSortingCumulated(new DhgSystemsNLogLogger()));
            controller.SortFile(fileToSortName, _sortedFileName);
            _logger.Info("Completed");
        }
    }
}