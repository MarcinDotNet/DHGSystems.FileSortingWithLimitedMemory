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
            const string charsAllowedForBegginig = "BCDEFGHIJKLMNOPQRSTUVWXYZbcdefghijklmnopqrstuvwxy";

            if (numberOfRows < 10)
            {
                throw new ArgumentException("Number of rows should be greater than 10");
            }

            var stringValues = new string[_diffValuesCount];
            var longTables = new long[_diffValuesCount];

            char[] chars = new char[_maxStringPartLenght];


            for (int i = 0; i < _diffValuesCount; i++)
            {
                chars[0] = charsAllowedForBegginig[Random.Shared.Next(charsAllowedForBegginig.Length)];

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
                long rowsToGen = numberOfRows;

                if (_addStaticTestData)
                {
                    rowsToGen = rowsToGen - 4;
                    outputFile.WriteLine("500. And i should be second ");
                    outputFile.WriteLine("500. Z I should be last ");
                }

                for (var i = 0; i < rowsToGen; i++)
                {
                    outputFile.WriteLine(longTables[Random.Shared.Next(_diffValuesCount)] + ". " + stringValues[Random.Shared.Next(_diffValuesCount)]);
                }
                outputFile.Write(longTables[Random.Shared.Next(_diffValuesCount)] + ". " + stringValues[Random.Shared.Next(_diffValuesCount)]);

                if (!_addStaticTestData) return;
                outputFile.WriteLine("1. And i should be first ");
                outputFile.WriteLine("1. Z I should be before last ");
            }
        }
    }
}
