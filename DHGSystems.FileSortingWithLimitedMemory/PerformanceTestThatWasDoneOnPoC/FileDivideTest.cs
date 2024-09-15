//using System.Diagnostics;
//using System.Globalization;


//namespace DHG.BigDataSorter.UnitTest;


//[TestClass]
//public class UnitTest2
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
//    private static string testFileName = test10GB;
//    /// <summary>
//    /// max row lenght = 1024, avarage 512,  1 000 0000 KB/0,5KB  = 2 000 000  
//    /// </summary>
//    private int maxLineCountPerFile = 1000000;

//    [TestMethod]
//    public void ReadLineSaveLineTest()
//    {
//        var watch = System.Diagnostics.Stopwatch.StartNew();
//        Process proc = Process.GetCurrentProcess();

//        using (StreamReader sr = File.OpenText(testFileName))
//        {
//            using (StreamWriter outputFile = new StreamWriter(Path.Combine(tempPathDivide, $"bigfile_ReadLineSaveLineTest_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}_copy.txt")))
//            {
//                string s = String.Empty;
//                long rowCount = 0;
//                while ((s = sr.ReadLine()) != null)
//                {
//                    outputFile.WriteLine(s);
//                    rowCount++;
//                }
//            }
//        }
//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString("N1") + $" File number   memory " + proc.PrivateMemorySize64.ToString("N1"));
//    }

//    [TestMethod]
//    public void ReadFullFileSaveFullFileTest()
//    {
//        var watch = System.Diagnostics.Stopwatch.StartNew();
//        Process proc = Process.GetCurrentProcess();

//        File.WriteAllLines(Path.Combine(tempPathDivide, $"bigfile_ReadLineSaveLineTest_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}_copy.txt"),File.ReadAllLines(testFileName));


//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString("N1") + $" File number   memory " + proc.PrivateMemorySize64.ToString("N1"));
//    }

//    [TestMethod]
//    public void ReadFullFileSaveFullFileTestBytes()
//    {
//        var watch = System.Diagnostics.Stopwatch.StartNew();
//        Process proc = Process.GetCurrentProcess();

//        File.WriteAllBytes(Path.Combine(tempPathDivide, $"bigfile_ReadLineSaveLineTest_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}_copy.txt"),File.ReadAllBytes(testFileName));


//        proc.Refresh();
//        Console.WriteLine(watch.ElapsedMilliseconds.ToString("N1") + $" File number   memory " + proc.PrivateMemorySize64.ToString("N1"));
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
//}