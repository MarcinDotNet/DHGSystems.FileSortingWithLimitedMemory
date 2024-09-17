using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort;

namespace DHGSystems.FileSortingWithLimitedMemory.UnitTests
{
    [TestClass]
    public class FileMergerWithSortingCumulatedTest
    {
        private readonly string tempPath = "MergedFilesOutput";
        private readonly string oneRowTestFile = @"TestFiles\\oneLineFile.txt";
        private readonly string oneRowTestResultFile = @"TestFilesSorted\\oneLineFileSorted.txt";
        private readonly string emailTestResultFile = @"TestFilesSorted\\emailTestSorted.txt";
        private readonly string emailTestResultFileCopy = @"TestFilesSorted\\emailTestSortedCopy.txt";
        private readonly string outputFilePath = @"MergedFilesOutput\\sortedFile.txt";

        [TestMethod]
        public void ProcessOneFile_Should_BePositive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);

            FileMergerWithSortingCumulated merger = new FileMergerWithSortingCumulated(new DhgSystemsNLogLogger());
            merger.MergeFilesWithSort(new string[] { oneRowTestFile }, outputFilePath, false);
            var fileContent = File.ReadAllText(outputFilePath);
            var resultFileContent = File.ReadAllText(oneRowTestResultFile);
            Assert.AreEqual(resultFileContent, fileContent);
        }

        [TestMethod]
        public void ProcessMultiLineRowFile_Should_BePositive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);

            FileMergerWithSortingCumulated merger = new FileMergerWithSortingCumulated(new DhgSystemsNLogLogger());
            merger.MergeFilesWithSort(new string[] { emailTestResultFile }, outputFilePath, false);
            var fileContent = File.ReadAllText(outputFilePath);
            var resultFileContent = File.ReadAllText(emailTestResultFileCopy);
            Assert.AreEqual(resultFileContent, fileContent);
        }

        [TestMethod]
        public void Process_Multiple_Files_With_The_Same_Values_Should_Be_Positive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);

            FileMergerWithSortingCumulated merger = new FileMergerWithSortingCumulated(new DhgSystemsNLogLogger());
            merger.MergeFilesWithSort(new string[] { emailTestResultFile, oneRowTestFile }, outputFilePath, false);
            var fileContent = File.ReadAllLines(outputFilePath);
            Assert.AreEqual(6, fileContent.Length);
        }
    }
}