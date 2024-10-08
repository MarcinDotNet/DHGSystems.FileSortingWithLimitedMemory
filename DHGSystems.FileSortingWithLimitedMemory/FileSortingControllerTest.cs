using DHGSystems.FileSortingWithLimitedMemory.Common;
using DHGSystems.FileSortingWithLimitedMemory.Common.Helpers;
using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Configuration;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Controllers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort;
using DHGSystems.FileSortingWithLimitedMemory.Lib.TestDataGenerator;
using System.Diagnostics;

namespace DHGSystems.FileSortingWithLimitedMemory.UnitTests
{
    [TestClass]
    public class FileSortingControllerTest
    {
        private readonly string tempPath = "SortingTempFolder";
        private readonly string oneRowTestFile = @"TestFiles\\oneLineFile.txt";
        private readonly string emailTestFile = @"TestFiles\\EmailTest.txt";
        private readonly string oneRowTestResultFile = @"TestFilesSorted\\oneLineFileSorted.txt";
        private readonly string emailTestResultFile = @"TestFilesSorted\\emailTestSorted.txt";
        private readonly string sortedFileName = @"SortingTempFolder\SortedFile.txt";
        private readonly string fileToSortName = @"SortingTempFolder\FileToSort.txt";

        [TestMethod]
        public void ProcesOneRowFile_Should_BePositive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);
            var configuration = GetFileSortedAppConfig();
            var oneThreadFileDivider = new OneThreadFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());
            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), oneThreadFileDivider, new SimpleFileMergerWithSorting());
            controller.SortFile(oneRowTestFile, sortedFileName, false);
            var fileContent = File.ReadAllText(sortedFileName);
            var resultFileContent = File.ReadAllText(oneRowTestResultFile);
            Assert.AreEqual(resultFileContent, fileContent);
        }

        [TestMethod]
        public void ProcesMultiLineRowFile_Should_BePositive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);
            var configuration = GetFileSortedAppConfig();
            var oneThreadFileDivider = new OneThreadFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());
            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), oneThreadFileDivider, new SimpleFileMergerWithSorting());
            controller.SortFile(emailTestFile, sortedFileName, false);
            var fileContent = File.ReadAllText(sortedFileName);
            var resultFileContent = File.ReadAllText(emailTestResultFile);
            Assert.AreEqual(resultFileContent, fileContent);
        }

        [Ignore]
        [TestMethod]
        public void Process_1GB_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watcher = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(17000000, fileToSortName);
            Console.WriteLine($"File generated in {watcher.ElapsedMilliseconds} ms");

            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 5000000;
            var oneThreadFileDivider = new OneThreadFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());
            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), oneThreadFileDivider, new SimpleFileMergerWithSorting());
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName, 1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual(Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile, secondLine);
        }

        [TestMethod]
        public void Process_1GB_Multi_Thread_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watcher = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(17000000, fileToSortName);
            Console.WriteLine($"File generated in {watcher.ElapsedMilliseconds} ms");

            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 1500000;
            var fileDivider = new MultiWorkersFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), fileDivider, new SimpleFileMergerWithSorting());
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName, 1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual(Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile, secondLine);
        }

        [TestMethod]
        public void Process_1GB_Multi_Thread_Cumulative_Merge_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watcher = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(17000000, fileToSortName);
            Console.WriteLine($"File generated in {watcher.ElapsedMilliseconds} ms");

            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 1500000;
            var fileDivider = new MultiWorkersFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), fileDivider, new FileMergerWithSortingCumulated(new DhgSystemsNLogLogger()));
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName, 1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual(Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile, secondLine);
        }

        [Ignore]
        [TestMethod]
        public void Process_10GB_Multi_Thread_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watcher = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(17000000 * 10, fileToSortName);
            Console.WriteLine($"File generated in {watcher.ElapsedMilliseconds} ms");

            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 1500000;
            var fileDivider = new MultiWorkersFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), fileDivider, new SimpleFileMergerWithSorting());
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName, 1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual(Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile, secondLine);
        }
        [Ignore]
        [TestMethod]
        public void Process_10GB_Multi_Thread_Cumulative_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watcher = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(17000000 * 10, fileToSortName);
            Console.WriteLine($"File generated in {watcher.ElapsedMilliseconds} ms");

            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 1500000;
            var fileDivider = new MultiWorkersFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), fileDivider, new FileMergerWithSortingCumulated(new DhgSystemsNLogLogger()));
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName, 1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual(Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile, secondLine);
        }
        [Ignore]
        [TestMethod]
        public void Process_100GB_Multi_Thread_Cumulative_Small_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watcher = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(17000000 * 100, fileToSortName);
            Console.WriteLine($"File generated in {watcher.ElapsedMilliseconds} ms");

            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 1500000;
            var fileDivider = new MultiWorkersFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), fileDivider, new SimpleFileMergerWithSorting());
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName, 1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual(Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile, secondLine);
        }

        [Ignore]
        [TestMethod]
        public void Process_100GB_Multi_Thread_Small_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watcher = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(17000000 * 100, fileToSortName);
            Console.WriteLine($"File generated in {watcher.ElapsedMilliseconds} ms");

            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 1500000;
            var fileDivider = new MultiWorkersFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), fileDivider, new SimpleFileMergerWithSorting());
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName, 1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual(Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile, secondLine);
        }

        [Ignore]
        [TestMethod]
        public void Process_100GB_Multi_Thread_Big_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watcher = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(17000000 * 100, fileToSortName);
            Console.WriteLine($"File generated in {watcher.ElapsedMilliseconds} ms");

            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 5000000;
            var fileDivider = new MultiWorkersFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), fileDivider, new SimpleFileMergerWithSorting());
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName, 1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual(Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile, secondLine);
        }

        [Ignore]
        [TestMethod]
        public void Process_1GB_Multi_Thread_BufforedMerge_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watcher = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(17000000, fileToSortName);
            Console.WriteLine($"File generated in {watcher.ElapsedMilliseconds} ms");

            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 5000000;
            var fileDivider = new MultiWorkersFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), fileDivider, new SimpleFileMergerWithSorting());
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName, 1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual(Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile, secondLine);
        }

        [Ignore]
        [TestMethod]
        public void Process_1GB_Multi_Thread_Async_Merge_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watcher = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(17000000, fileToSortName);
            Console.WriteLine($"File generated in {watcher.ElapsedMilliseconds} ms");

            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 5000000;
            var fileDivider = new MultiWorkersFileDivider(configuration.TempFolderPath, configuration.SortedFilePrefix, new DhgSystemsNLogLogger());

            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger(), fileDivider, new FileMergerWithSortingWithAsyncWrite());
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName, 1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual(Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile, secondLine);
        }

        public FileSortingAppConfiguration GetFileSortedAppConfig()
        {
            return new FileSortingAppConfiguration()
            {
                MaxLinesBeforeSort = 500000,
                SortedFilePrefix = "sorted_file_",
                TempFolderPath = tempPath
            };
        }
    }
}