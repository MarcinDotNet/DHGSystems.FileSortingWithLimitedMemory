using DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort;

namespace DHGSystems.FileSortingWithLimitedMemory
{
    [TestClass]
    public class SimpleFileMergerWithSortingTest
    {
        private readonly string tempPath = "MergedFilesOutput";
        private readonly string oneRowTestFile = @"TestFiles\\oneLineFile.txt";
        private readonly string oneRowTestResultFile = @"TestFilesSorted\\oneLineFileSorted.txt";
        private readonly string emailTestResultFile = @"TestFilesSorted\\emailTestSorted.txt";
        private readonly string outputFilePath = @"MergedFilesOutput\\sortedFile.txt";

        [TestMethod]
        public void ProcessOneFile_Should_BePositive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);

            SimpleFileMergerWithSorting simpleFileMergerWithSorting = new SimpleFileMergerWithSorting();
            simpleFileMergerWithSorting.MergeFilesWithSort(new string[] { oneRowTestFile }, outputFilePath);
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

            SimpleFileMergerWithSorting simpleFileMergerWithSorting = new SimpleFileMergerWithSorting();
            simpleFileMergerWithSorting.MergeFilesWithSort(new string[] { emailTestResultFile }, outputFilePath);
            var fileContent = File.ReadAllText(outputFilePath);
            var resultFileContent = File.ReadAllText(emailTestResultFile);
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

            SimpleFileMergerWithSorting simpleFileMergerWithSorting = new SimpleFileMergerWithSorting();
            simpleFileMergerWithSorting.MergeFilesWithSort(new string[] { emailTestResultFile, emailTestResultFile, emailTestResultFile, emailTestResultFile, emailTestResultFile, emailTestResultFile }, outputFilePath);
            var fileContent = File.ReadAllLines(outputFilePath);
            var resultFileContent = File.ReadAllLines(emailTestResultFile);
            Assert.AreEqual(resultFileContent.Length * 6, fileContent.Length);
        }
    }
}