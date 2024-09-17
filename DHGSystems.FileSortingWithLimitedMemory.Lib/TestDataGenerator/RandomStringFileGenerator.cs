using DHGSystems.FileSortingWithLimitedMemory.Common;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.TestDataGenerator
{
    public class RandomStringFileGenerator : ITestDataFileGenerator
    {
        private readonly int _maxStringPartLenght;
        private readonly int _diffValuesCount;
        private readonly bool _addStaticTestData;

        public RandomStringFileGenerator(int maxStringLength, int diffValuesCount, bool addStaticTestData)
        {
            this._maxStringPartLenght = maxStringLength;
            this._diffValuesCount = diffValuesCount;
            this._addStaticTestData = addStaticTestData;

            if (maxStringLength < 2)
            {
                throw new ArgumentException("Max string length should be greater than 1");
            }

            if (diffValuesCount < 100)
            {
                throw new ArgumentException("Different values count should be greater than 100");
            }
        }

        public void GenerateTestFile(long numberOfRows, string outputFileFullFileName)
        {
            const string charsAllAllowed = "ABCDEFGHIJKLM NOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const string charsAllowedForBeggining = "BCDEFGHIJKLMNOPQRSTUVWXYbcdefghijklmnopqrstuvwxy";

            if (numberOfRows < 10)
            {
                throw new ArgumentException("Number of rows should be greater than 10");
            }

            var stringValues = new string[_diffValuesCount];
            var longTables = new long[_diffValuesCount];

            char[] chars = new char[_maxStringPartLenght];

            for (int i = 0; i < _diffValuesCount; i++)
            {
                chars[0] = charsAllowedForBeggining[Random.Shared.Next(charsAllowedForBeggining.Length)];

                int currentStringLenght = Random.Shared.Next(2, _maxStringPartLenght);

                for (int j = 1; j < currentStringLenght; j++)
                {
                    chars[j] = charsAllAllowed[Random.Shared.Next(charsAllAllowed.Length)];
                }

                stringValues[i] = new string(chars[0..currentStringLenght]);
                longTables[i] = Random.Shared.NextInt64(1, int.MaxValue);
            }

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(outputFileFullFileName))
            {
                outputFile.AutoFlush = false;
                long rowsToGen = numberOfRows;

                if (_addStaticTestData)
                {
                    rowsToGen = rowsToGen - 4;
                    outputFile.WriteLine(Constants.SecondLineInTestFile);
                    outputFile.WriteLine(Constants.LastLineInTestFile);
                }
                int flushCount = 0;
                for (var i = 0; i < rowsToGen; i++)
                {
                    outputFile.WriteLine(longTables[Random.Shared.Next(_diffValuesCount)] + ". " + stringValues[Random.Shared.Next(_diffValuesCount)]);
                    flushCount++;
                    if (flushCount == 50000)
                    {
                        outputFile.Flush();
                        flushCount = 0;
                    }
                }
                outputFile.Write(longTables[Random.Shared.Next(_diffValuesCount)] + ". " + stringValues[Random.Shared.Next(_diffValuesCount)]);

                if (!_addStaticTestData) return;
                outputFile.WriteLine();
                outputFile.WriteLine(Constants.FirstLineInTestFile);
                outputFile.Write(Constants.BeforeLastLineInTestFile);
                outputFile.Flush();
            }
        }
    }
}