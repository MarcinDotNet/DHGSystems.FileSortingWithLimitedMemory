using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.Configuration
{
    public class FilesSortedAppConfiguration
    {
        public string MergeFolderPath { get; set; }
        public string TempFolderPath { get; set; }
        public string OutputPath { get; set; }
        public long MaxLinesBeforeSort { get; set; }
    }
}
