using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers;


namespace DHGSystems.FileSortingWithLimitedMemory
{
    [TestClass]
    public class MultiWorkersFileDividerTest
    {
        private readonly string tempPath = "MultiWorkersFileDividerTestFolder";
        private readonly string testFolderPath = @"TestFiles\\";
        private readonly string oneRowTestFile = @"TestFiles\\oneLineFile.txt";
        private readonly string emailTestFile = @"TestFiles\\EmailTest.txt";
        private readonly string oneRowTestResultFile = @"TestFilesSorted\\oneLineFileSorted.txt";
        private readonly string emailTestResultFile = @"TestFilesSorted\\emailTestSorted.txt";

        [TestMethod]
        public void ProcesOneRowFile_Should_BePositive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);
            MultiWorkersFileDivider MultiWorkersFileDivider =
                new MultiWorkersFileDivider(tempPath, "sorted_file_", new DhgSystemsNLogLogger());
            var generatedFiles = MultiWorkersFileDivider.DivideFileWithSort(oneRowTestFile, 5).ToList();
            var fileContent = File.ReadAllText(generatedFiles.First());
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
            MultiWorkersFileDivider MultiWorkersFileDivider =
                new MultiWorkersFileDivider(tempPath, "sorted_file_", new DhgSystemsNLogLogger());
            var generatedFiles = MultiWorkersFileDivider.DivideFileWithSort(emailTestFile, 40).ToList();
            var fileContent = File.ReadAllText(generatedFiles.First());
            var resultFileContent = File.ReadAllText(emailTestResultFile);
            Assert.AreEqual(resultFileContent, fileContent);
        }

        [TestMethod]
        public void ProcesMultiLineRowFile_OnLineCount_Equal_MaxBatch_Should_BePositive()
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);
            MultiWorkersFileDivider MultiWorkersFileDivider =
                new MultiWorkersFileDivider(tempPath, "sorted_file_", new DhgSystemsNLogLogger());
            var generatedFiles = MultiWorkersFileDivider.DivideFileWithSort(emailTestFile, 40).ToList();
            var fileContent = File.ReadAllText(generatedFiles.First());
            var resultFileContent = File.ReadAllText(emailTestResultFile);
            Assert.AreEqual(resultFileContent, fileContent);
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
            MultiWorkersFileDivider MultiWorkersFileDivider =
                new MultiWorkersFileDivider(tempPath, "sorted_file_", new DhgSystemsNLogLogger());
            var generatedFiles = MultiWorkersFileDivider.DivideFileWithSort(emailTestFile, maxLineCount).ToList();
            Assert.AreEqual(expectedFileCount, generatedFiles.Count);
        }
    }
}