using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.TestDataGenerator
{
    public interface ITestDataGenerator
    {
        void GenerateTestFile(long numberOfRows, int averageStringSize, string outputFileFullFileName);
    }
}