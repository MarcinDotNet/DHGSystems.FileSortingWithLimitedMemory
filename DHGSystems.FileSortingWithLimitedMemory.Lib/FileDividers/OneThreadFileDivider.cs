using DHGSystems.FileSortingWithLimitedMemory.Lib.Model;
using System.Diagnostics;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers
{
    public class OneThreadFileDivider : IFileDividerWithSort
    {
        private string tempPathSort = String.Empty;
        public IEnumerable<string> DivideFileWithSort(string fileToDived, long maxLinesBeforeSort)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Process proc = Process.GetCurrentProcess();
            Console.WriteLine(proc.PrivateMemorySize64.ToString("N1")); ;

            using (StreamReader sr = File.OpenText(fileToDived))
            {
                int position;
                int lineCount = 0;
                int fileNumber = 1;
                string[] allStrings = new string[maxLinesBeforeSort];
                string lineText = String.Empty;
                BigDataEntryRef[] loadedValues = new BigDataEntryRef[maxLinesBeforeSort];
                while ((lineText = sr.ReadLine()) != null)
                {
                    position = lineText.IndexOf(".");
                    allStrings[lineCount] = lineText.Substring(position + 1);
                    loadedValues[lineCount].Number = long.Parse(lineText.Substring(0, position));
                    loadedValues[lineCount].Name = lineCount;
                    lineCount++;

                    if (lineCount == maxLinesBeforeSort)
                    {
                        proc.Refresh();
                        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" File number {fileNumber}  rows read " + proc.PrivateMemorySize64.ToString("N1"));

                        using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathSort, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
                        {
                            var sorted = loadedValues.AsParallel().OrderBy(x => allStrings[x.Name]).ThenBy(y => y.Number).ToArray();
                            Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1")); ;
                            for (int i = 0; i < sorted.Length; i++)
                            {
                                outputFile.Write(sorted[i].Number);
                                outputFile.Write(".");
                                outputFile.WriteLine(allStrings[sorted[i].Name]);


                            }
                        }
                        proc.Refresh();
                        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" File number {fileNumber} file saved " + proc.PrivateMemorySize64.ToString("N1")); ;
                        GC.Collect();
                        fileNumber++;
                        lineCount = 0;
                    }
                }
                proc.Refresh();
                Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" Rest loaded {fileNumber} " + proc.PrivateMemorySize64.ToString("N1")); ;
                if (lineCount > 0)
                {
                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathSort, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
                    {
                        var sorted = loadedValues[0..lineCount].AsParallel().OrderBy(x => allStrings[x.Name]).ThenBy(y => y.Number).ToArray();
                        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1")); ;
                        for (int i = 0; i < sorted.Length; i++)
                        {
                            outputFile.Write(sorted[i].Number);
                            outputFile.Write(".");
                            outputFile.WriteLine(allStrings[sorted[i].Name]);
                        }
                    }
                };

                proc.Refresh();
                Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" Rest sorted and saved {fileNumber} " + proc.PrivateMemorySize64.ToString("N1")); ;
            }

            proc.Refresh();
            Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Total:  " + proc.PrivateMemorySize64.ToString("C"));
            return null;
        }
    }
}
