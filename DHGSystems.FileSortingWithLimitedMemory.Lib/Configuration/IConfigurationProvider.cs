using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.Configuration
{
    public interface IConfigurationProvider
    {
        FilesSortedAppConfiguration GetConfiguration();
    }
}
