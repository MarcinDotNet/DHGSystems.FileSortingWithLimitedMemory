using DHGSystems.FileSortingWithLimitedMemory.Common;
using DHGSystems.FileSortingWithLimitedMemory.Common.Helpers;
using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Configuration;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Controllers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.TestDataGenerator;


namespace DHGSystems.FileSortingWithLimitedMemory
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
            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger());
            controller.SortFile(oneRowTestFile, sortedFileName);
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
            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger());
            controller.SortFile(emailTestFile, sortedFileName);
            var fileContent = File.ReadAllText(sortedFileName);
            var resultFileContent = File.ReadAllText(emailTestResultFile);
            Assert.AreEqual(resultFileContent, fileContent);
        }

        [TestMethod]
        public void Process_1GB_file_should_Be_positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(100, 50000, true);
            randomStringFileGenerator.GenerateTestFile(15000000, fileToSortName);
            
            var configuration = GetFileSortedAppConfig();
            configuration.MaxLinesBeforeSort = 5000000;
            FileSortingController controller = new FileSortingController(configuration, new DhgSystemsNLogLogger());
            controller.SortFile(fileToSortName, sortedFileName);
            Console.WriteLine(ProcessHelper.GetUsedMemoryInMb());
            var firstLine = FileHelper.GetLineContentFromFile(sortedFileName,1);
            var secondLine = FileHelper.GetLineContentFromFile(sortedFileName, 2);
            Assert.AreEqual( Constants.FirstLineInTestFile, firstLine);
            Assert.AreEqual(Constants.SecondLineInTestFile,secondLine);
        }




        [DataTestMethod]
        [DataRow(1, 5)]
        [DataRow(2, 3)]
        [DataRow(3, 2)]
        [DataRow(4, 2)]
        [DataRow(5, 1)]
        [DataRow(6, 1)]

        public void ProcesMultiLineFile_WithDifferent_DivideLineCount_Should_BePositive(long maxLineCount,
            int expectedFileCount)
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);
            OneThreadFileDivider oneThreadFileDivider =
                new OneThreadFileDivider(tempPath, "sorted_file_", new DhgSystemsNLogLogger());
            var generatedFiles = oneThreadFileDivider.DivideFileWithSort(emailTestFile, maxLineCount).ToList();
            Assert.AreEqual(expectedFileCount, generatedFiles.Count);
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