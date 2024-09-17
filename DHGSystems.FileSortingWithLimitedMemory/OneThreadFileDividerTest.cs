using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers;
using System.Collections.Concurrent;

namespace DHGSystems.FileSortingWithLimitedMemory.UnitTests
{
    [TestClass]
    public class OneThreadFileDividerTest
    {
        private readonly string tempPath = "OneThreadFileDividerTestFolder";
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
            ConcurrentQueue<string> filesProcessed = new ConcurrentQueue<string>();
            OneThreadFileDivider oneThreadFileDivider =
                new OneThreadFileDivider(tempPath, "sorted_file_", new DhgSystemsNLogLogger());
            oneThreadFileDivider.DivideFileWithSort(oneRowTestFile, 5, filesProcessed);
            var generatedFiles = filesProcessed.ToList();
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
            ConcurrentQueue<string> filesProcessed = new ConcurrentQueue<string>();
            OneThreadFileDivider oneThreadFileDivider =
                new OneThreadFileDivider(tempPath, "sorted_file_", new DhgSystemsNLogLogger());
            oneThreadFileDivider.DivideFileWithSort(emailTestFile, 40, filesProcessed);
            var generatedFiles = filesProcessed.ToList();
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
            ConcurrentQueue<string> filesProcessed = new ConcurrentQueue<string>();
            OneThreadFileDivider oneThreadFileDivider =
                new OneThreadFileDivider(tempPath, "sorted_file_", new DhgSystemsNLogLogger());
            oneThreadFileDivider.DivideFileWithSort(emailTestFile, 40, filesProcessed);
            var generatedFiles = filesProcessed.ToList();
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
            ConcurrentQueue<string> filesProcessed = new ConcurrentQueue<string>();
            OneThreadFileDivider oneThreadFileDivider =
                new OneThreadFileDivider(tempPath, "sorted_file_", new DhgSystemsNLogLogger());
            oneThreadFileDivider.DivideFileWithSort(emailTestFile, maxLineCount, filesProcessed);
            var generatedFiles = filesProcessed.ToList();
            Assert.AreEqual(expectedFileCount, generatedFiles.Count);
        }
    }
}