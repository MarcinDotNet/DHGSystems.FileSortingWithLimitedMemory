using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers;


namespace DHGSystems.FileSortingWithLimitedMemory
{
    [TestClass]
    public class OneThreadFileDividerTest
    {
        private readonly string _tempPath = "OneThreadFileDividerTestFolder";

        [TestMethod]
        public void ProcesOneRowFile_Should_BePositive()
        {
            Directory.Delete(_tempPath, true);
            Directory.CreateDirectory(_tempPath);
            OneThreadFileDivider oneThreadFileDivider = new OneThreadFileDivider(_tempPath, 1, "sorted_file_", new DhgSystemsNLogLogger());
            var result = oneThreadFileDivider.DivideFileWithSort("OneRowFile.txt", 1);

        }
    }
}