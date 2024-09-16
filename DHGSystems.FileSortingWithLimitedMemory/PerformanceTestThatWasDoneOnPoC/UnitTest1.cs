//using System.Diagnostics;
//using System.Globalization;

//namespace DHG.BigDataSorter.UnitTest;

//[TestClass]
//public class UnitTest1
//{
//    private string path = @"D:\BigData";
//    private string tempPathSort = @"D:\BigData\TempSort";
//    private string tempPathDivide = @"D:\BigData\TempDivide";
//    private string testFileName1 = @"D:\BigData\bigfile_20240911_111352851.txt";
//    private static string test10GB = @"D:\BigData\file_10GB.txt";
//    private static string test20GB = @"D:\BigData\file_20GB.txt";
//    private static string test1GB = @"D:\BigData\file_1GB.txt";
//    private static string test2GB = @"D:\BigData\file_2GB.txt";
//    private static string test3GB = @"D:\BigData\file_3GB.txt";
//    private static string test05GB = @"D:\BigData\file_05GB.txt";
//    private static string testFileName = test2GB;
//    /// <summary>
//    /// max row lenght = 1024, avarage 512,  1 000 0000 KB/0,5KB  = 2 000 000
//    /// </summary>
//    private int maxLineCountPerFile = 20000000;
//    private int maxLineCountPerFileArr = 10000000;

//    [TestMethod]
//    public void GenerateFileMethod()
//    {
//        int arrSize = 100000;
//        var stringValues = new string[100000];
//        var longTables = new long[100000];
//        for (int i = 0; i < 100000; i++)
//        {
//            stringValues[i] = RandomString(Random.Shared.Next(4, 50));
//            longTables[i] = Random.Shared.NextInt64(1, long.MaxValue);
//        }
//        // Write the string array to a new file named "WriteLines.txt".
//        using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, $"bigfile_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//        {
//            for (int i = 0; i < 10040000; i++)
//            {
//                outputFile.WriteLine(longTables[Random.Shared.Next(arrSize / 10)] + "." + stringValues[Random.Shared.Next(arrSize)]);
//            }
//        }
//    }

//    [TestMethod]
//    public void SortFile()
//    {
//        var watch = System.Diagnostics.Stopwatch.StartNew();
//        Process proc = Process.GetCurrentProcess();
//        Console.WriteLine(proc.PrivateMemorySize64.ToString("N1"));;
//        var data = File.ReadAllLines(testFileName)
//            .Select(x => (long.Parse(x.Substring(0, x.IndexOf("."))), x.Substring(x.IndexOf(".") + 1)));
//        GC.Collect();
//        var sorted = data.OrderBy(x => x.Item2).ThenBy(x => x.Item1)
//            .ToArray();
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " " + proc.PrivateMemorySize64.ToString("N1"));;
//        using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//        {
//            foreach (var y in sorted)
//            {
//                outputFile.WriteLine(string.Concat(y.Item1, ".", y.Item2));
//            }
//        }
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " " + proc.PrivateMemorySize64.ToString("N1"));;
//    }

//    [TestMethod]
//    public void SortFileOptimalizedOrderBy()
//    {
//        var watch = System.Diagnostics.Stopwatch.StartNew();
//        Process proc = Process.GetCurrentProcess();
//        Console.WriteLine(proc.PrivateMemorySize64.ToString("N1"));;

//        List<BigDataEntry> data = new List<BigDataEntry>();
//        using (StreamReader sr = File.OpenText(testFileName))
//        {
//            int position;
//            string s = String.Empty;
//            while ((s = sr.ReadLine()) != null)
//            {
//                position = s.IndexOf(".");
//                data.Add(new BigDataEntry() { Number = long.Parse(s.Substring(0, position)), Name = s.Substring(position) });
//            }
//        }
//        GC.Collect();
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files Read : " + proc.PrivateMemorySize64.ToString("N1"));;
//        GC.Collect();
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1"));;
//        using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathSort, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//        {
//            var enumerator = data.AsParallel().OrderBy(x => x.Name).ThenBy(y => y.Number).GetEnumerator();
//            proc.Refresh();
//            Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1"));;
//            while (enumerator.MoveNext())
//            {
//                //outputFile.WriteLine(enumerator.Current.Number + enumerator.Current.Name);
//            }
//        }
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " File saved: " + proc.PrivateMemorySize64.ToString("N1"));;
//    }

//    [TestMethod]
//    public void SortFileOptimalizedOrderByWithStringIndex()
//    {
//        var watch = System.Diagnostics.Stopwatch.StartNew();
//        Process proc = Process.GetCurrentProcess();
//        Console.WriteLine(proc.PrivateMemorySize64.ToString("N1"));;
//        string[] allStrings = new string[maxLineCountPerFileArr*10];
//        List<BigDataEntryRef> data = new List<BigDataEntryRef>();
//        using (StreamReader sr = File.OpenText(testFileName))
//        {
//            int position;
//            int counter = 0;
//            string s = String.Empty;
//            while ((s = sr.ReadLine()) != null)
//            {
//                position = s.IndexOf(".");
//                allStrings[counter] = s.Substring(position);
//                data.Add(new BigDataEntryRef() { Number = long.Parse(s.Substring(0, position)), Name = counter});
//                counter++;
//            }
//        }
//        GC.Collect();
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files Read : " + proc.PrivateMemorySize64.ToString("N1"));;
//        GC.Collect();
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1"));;
//        using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathSort, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//        {
//            var sorted = data.AsParallel().OrderBy(x => allStrings[x.Name]).ThenBy(y => y.Number).ToArray();
//            Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1"));;
//            for (int i = 0; i < sorted.Length; i++)
//            {
//                outputFile.Write(allStrings[sorted[i].Name]);
//                outputFile.WriteLine(sorted[i].Number);
//            }
//        }

//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " File saved: " + proc.PrivateMemorySize64.ToString("N1"));;
//    }

//    [TestMethod]
//    public void SortFileOptimalizedSort1()
//    {
//        var watch = System.Diagnostics.Stopwatch.StartNew();
//        Process proc = Process.GetCurrentProcess();
//        Console.WriteLine(proc.PrivateMemorySize64.ToString("N1"));;

//        List<BigDataEntry> data = new List<BigDataEntry>();
//        using (StreamReader sr = File.OpenText(testFileName))
//        {
//            int position;
//            string s = String.Empty;
//            while ((s = sr.ReadLine()) != null)
//            {
//                position = s.IndexOf(".");
//                data.Add(new BigDataEntry() { Number = long.Parse(s.Substring(0, position)), Name = s.Substring(position) });
//            }
//        }
//        GC.Collect();
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files Read : " + proc.PrivateMemorySize64.ToString("N1"));;
//        GC.Collect();
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1"));;
//        using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathSort, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//        {
//            data.Sort();
//            proc.Refresh();
//            Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1"));;
//            for (int i = 0; i < data.Count(); i++)
//            {
//                outputFile.Write(data[i].Number);
//                outputFile.Write(data[i].Name);
//                outputFile.Write(Environment.NewLine);
//            }
//        }
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " File saved: " + proc.PrivateMemorySize64.ToString("N1"));;
//    }

//    [TestMethod]
//    public void DivideFileIntoParts()
//    {
//        var watch = System.Diagnostics.Stopwatch.StartNew();
//        Process proc = Process.GetCurrentProcess();
//        Console.WriteLine(proc.PrivateMemorySize64.ToString("N1"));;

//        using (StreamReader sr = File.OpenText(testFileName))
//        {
//            int position;
//            int lineCount = 0;
//            int fileNumber = 1;
//            string[] allStrings = new string[maxLineCountPerFileArr];
//            string lineText = String.Empty;
//            BigDataEntryRef[] loadedValues = new BigDataEntryRef[maxLineCountPerFileArr];
//            while ((lineText = sr.ReadLine()) != null)
//            {
//                position = lineText.IndexOf(".");
//                allStrings[lineCount] = lineText.Substring(position+1);
//                loadedValues[lineCount].Number = long.Parse(lineText.Substring(0, position));
//                loadedValues[lineCount].Name = lineCount;
//                lineCount++;

//                if (lineCount == maxLineCountPerFileArr)
//                {
//                    proc.Refresh();
//                    Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" File number {fileNumber}  rows read " + proc.PrivateMemorySize64.ToString("N1"));

//                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathSort, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//                    {
//                        var sorted = loadedValues.AsParallel().OrderBy(x => allStrings[x.Name]).ThenBy(y => y.Number).ToArray();
//                        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1"));;
//                        for (int i = 0; i < sorted.Length; i++)
//                        {
//                            outputFile.Write(sorted[i].Number);
//                            outputFile.Write(".");
//                            outputFile.WriteLine(allStrings[sorted[i].Name]);

//                        }
//                    }
//                    proc.Refresh();
//                    Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" File number {fileNumber} file saved " + proc.PrivateMemorySize64.ToString("N1"));;
//                    GC.Collect();
//                    fileNumber++;
//                    lineCount = 0;
//                }
//            }
//            proc.Refresh();
//            Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" Rest loaded {fileNumber} " + proc.PrivateMemorySize64.ToString("N1"));;
//            if (lineCount > 0)
//            {
//                using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathSort, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//                {
//                    var sorted = loadedValues[0..lineCount].AsParallel().OrderBy(x => allStrings[x.Name]).ThenBy(y => y.Number).ToArray();
//                    Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1"));;
//                    for (int i = 0; i < sorted.Length; i++)
//                    {
//                        outputFile.Write(sorted[i].Number);
//                        outputFile.Write(".");
//                        outputFile.WriteLine(allStrings[sorted[i].Name]);
//                    }
//                }
//            };

//            proc.Refresh();
//            Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" Rest sorted and saved {fileNumber} " + proc.PrivateMemorySize64.ToString("N1"));;
//        }

//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Total:  " + proc.PrivateMemorySize64.ToString("C"));
//    }

//    [TestMethod]
//    public void DivideFileIntoPartsV2()
//    {
//        var watch = System.Diagnostics.Stopwatch.StartNew();
//        Process proc = Process.GetCurrentProcess();
//        Console.WriteLine(proc.PrivateMemorySize64.ToString("N1"));;

//        using (StreamReader sr = File.OpenText(testFileName))
//        {
//            int position;
//            int lineCount = 0;
//            int fileNumber = 1;
//            string lineText = String.Empty;
//            BigDataEntry[] loadedValues = new BigDataEntry[maxLineCountPerFileArr];
//            while ((lineText = sr.ReadLine()) != null)
//            {
//                position = lineText.IndexOf(".");
//                loadedValues[lineCount].Number = long.Parse(lineText.Substring(0, position));
//                loadedValues[lineCount].Name = lineText.Substring(position + 1);
//                lineCount++;

//                if (lineCount == maxLineCountPerFileArr)
//                {
//                    proc.Refresh();
//                    Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" File number {fileNumber}  rows read " + proc.PrivateMemorySize64.ToString("N1"));

//                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathSort, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//                    {
//                        var sorted = loadedValues.AsParallel().OrderBy(x => x.Name).ThenBy(y => y.Number).ToArray();
//                        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1"));;
//                        for (int i = 0; i < sorted.Length; i++)
//                        {
//                            outputFile.Write(sorted[i].Number);
//                            outputFile.Write(".");
//                            outputFile.WriteLine(sorted[i].Name);

//                        }
//                    }

//                    proc.Refresh();
//                    Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" File number {fileNumber} file saved " + proc.PrivateMemorySize64.ToString("N1"));;
//                    GC.Collect();
//                    fileNumber++;
//                    lineCount = 0;
//                }
//            }
//            proc.Refresh();
//            Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" Rest loaded {fileNumber} " + proc.PrivateMemorySize64.ToString("N1"));;
//            if (lineCount > 0)
//            {
//                using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathSort, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//                {
//                    var sorted = loadedValues[0..lineCount].AsParallel().OrderBy(x => x.Name).ThenBy(y => y.Number).ToArray();
//                    Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Files sorted: " + proc.PrivateMemorySize64.ToString("N1"));;
//                    for (int i = 0; i < sorted.Length; i++)
//                    {
//                        outputFile.WriteLine(sorted[i].Name);
//                        outputFile.WriteLine(".");
//                        outputFile.WriteLine(sorted[i].Number);
//                        outputFile.WriteLine(Environment.NewLine);
//                    }
//                }
//            };

//            proc.Refresh();
//            Console.WriteLine(watch.ElapsedMilliseconds.ToString() + $" Rest sorted and saved {fileNumber} " + proc.PrivateMemorySize64.ToString("N1"));;
//        }

//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " Total:  " + proc.PrivateMemorySize64.ToString("C"));
//    }

//    private void SortAndSaveData(BigDataEntry[] data, long fileNumber)
//    {
//        using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathDivide, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}_{fileNumber}.txt")))
//        {
//            data.OrderBy(x => x.Name).ThenBy(x => x.Number).ToList()
//                .ForEach(x => outputFile.WriteLine(string.Concat(x.Number.ToString(), x.Name)));
//          //  for (int i = 0; i < data.Length; i++)
//           // {
//            //    outputFile.WriteLine(string.Concat(sorted[i].Number, sorted[i].Name));
//         //   }
//        }
//    }

//    [TestMethod]
//    public void SortFileOptimalizedSort()
//    {
//        var watch = System.Diagnostics.Stopwatch.StartNew();
//        Process proc = Process.GetCurrentProcess();
//        Console.WriteLine(proc.PrivateMemorySize64.ToString("N1"));;

//        List<BigDataEntry> data = new List<BigDataEntry>();
//        using (StreamReader sr = File.OpenText(testFileName))
//        {
//            int position;
//            string s = String.Empty;
//            while ((s = sr.ReadLine()) != null)
//            {
//                position = s.IndexOf(".");
//                data.Add(new BigDataEntry() { Number = long.Parse(s.Substring(0, position)), Name = s.Substring(position) });
//            }
//        }
//        // GC.Collect();
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " " + proc.PrivateMemorySize64.ToString("N1"));;
//        data.Sort();
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " " + proc.PrivateMemorySize64.ToString("N1"));;
//        using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, $"bigfile_sorted_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//        {
//            foreach (var y in data)
//            {
//                outputFile.WriteLine(string.Concat(y.Number, ".", y.Name));
//            }
//        }
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " " + proc.PrivateMemorySize64.ToString("N1"));;
//    }

//    public static string RandomString(int length)
//    {
//        const string chars = "ABCDEFGHIJKLM NOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
//        return new string(Enumerable.Repeat(chars, length)
//            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
//    }

//    public struct BigDataEntry : IEquatable<BigDataEntry>, IComparable<BigDataEntry>
//    {
//        public long Number { get; set; }

//        public string Name { get; set; }

//        public bool Equals(BigDataEntry other)
//        {
//            return other.Name == Name && other.Number == Number;
//        }

//        public int CompareTo(BigDataEntry other)
//        {
//            var nameCompare = Name.CompareTo(other.Name);
//            return nameCompare != 0 ? nameCompare : Number.CompareTo(other.Number);
//        }
//    }

//    public struct BigDataEntryRef
//    {
//        public long Number { get; set; }

//        public long Name { get; set; }
//    }
//}