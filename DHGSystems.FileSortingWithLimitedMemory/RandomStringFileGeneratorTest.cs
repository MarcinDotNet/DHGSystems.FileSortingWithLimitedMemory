using DHGSystems.FileSortingWithLimitedMemory.Lib.TestDataGenerator;
using System.Diagnostics;
using DHGSystems.FileSortingWithLimitedMemory.Common.Helpers;


namespace DHGSystems.FileSortingWithLimitedMemory
{
    [TestClass]
    public class RandomStringFileGeneratorTest
    {
        private readonly string tempPath = "GeneratedFilesTempFolder";
        private readonly string testFileName = @"GeneratedFilesTempFolder\testfile.txt";

        [DataTestMethod]
        [DataRow(100, 100, true, 1000)]
        [DataRow(1000, 1000, true, 10000)]
        [DataRow(50, 5000, true, 1000000)]
        [DataRow(50, 5000, true, 10000000)]
       // [DataRow(100, 5000, true, 100000000)]


        public void Generate_Files_With_For_Multiple_Parameters_Should_BePositive(int maxStringLength,
            int diffCountValues, bool useStaticData, long lineCount)
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);
            var watch = Stopwatch.StartNew();
            RandomStringFileGenerator randomStringFileGenerator = new RandomStringFileGenerator(maxStringLength, diffCountValues, true);
            randomStringFileGenerator.GenerateTestFile(lineCount, testFileName);

            Process proc = Process.GetCurrentProcess();
            Console.WriteLine($"Memory {(ProcessHelper.GetUsedMemoryInMb()):N1} MB");
            Console.WriteLine($"Time {watch.ElapsedMilliseconds:N1}");
            decimal fileSizesInMB = new FileInfo(testFileName).Length/1024;
            Console.WriteLine($"File size {fileSizesInMB.ToString($"N1")} KB");

            if (lineCount < 23000)
            {
                var generatedFileContent = File.ReadAllLines(testFileName);

                Assert.AreEqual(lineCount, generatedFileContent.Length);
                Assert.IsTrue(generatedFileContent.All(x => x.Contains(".")));

                if (useStaticData)
                {
                    Assert.IsTrue(generatedFileContent.Any(x => x.Contains("500. And i should be second")));
                    Assert.IsTrue(generatedFileContent.Any(x => x.Contains("500. Z I should be last")));
                }
            }
        }
    }
}