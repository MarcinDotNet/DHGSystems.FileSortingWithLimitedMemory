using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.TestDataGenerator;
using System.Diagnostics;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Configuration;
using DHGSystems.FileSortingWithLimitedMemory.Common.Helpers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Controllers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort;

namespace DHGSystems.FileSortingWithLimitedMemory.App
{
    internal class Program
    {
        private static string _tempPath = "SortingTempFolder";
        private static string _sortedFileName = @"SortingTempFolder\SortedFile.txt";
        private static string _fileToSortName = @"SortingTempFolder\FileToSort.txt";
        private static IDhgSystemsLogger _logger = new DhgSystemsNLogLogger();
        static void Main(string[] args)
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

                RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 500000, true);
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
                MaxLinesBeforeSort = 500000,
                SortedFilePrefix = "sorted_file_",
                TempFolderPath = _tempPath
            };
            
            fileSortingAppConfiguration.MaxLinesBeforeSort = 1500000;
            var fileDivider = new MultiWorkersFileDivider(fileSortingAppConfiguration.TempFolderPath, fileSortingAppConfiguration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(fileSortingAppConfiguration, new DhgSystemsNLogLogger(), fileDivider, new SimpleFileMergerWithSorting());
            controller.SortFile(fileToSortName, _sortedFileName);
            _logger.Info("Completed");
        }
    }
}
