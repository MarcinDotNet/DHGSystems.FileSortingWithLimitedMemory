namespace DHGSystems.FileSortingWithLimitedMemory.Lib.TestDataGenerator
{
    public interface ITestDataFileGenerator
    {
        void GenerateTestFile(long numberOfRows, string outputFileFullFileName);
    }
}