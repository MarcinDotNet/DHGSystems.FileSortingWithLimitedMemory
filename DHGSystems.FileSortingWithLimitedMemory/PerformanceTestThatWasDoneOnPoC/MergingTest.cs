//namespace DHG.BigDataSorter.UnitTest;

//[TestClass]
//public class MergingTest
//{
//    private string path = @"D:\BigData";
//    private string mergePath = @"D:\BigData\MergeResults";
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
//        List<string> filesToProcess = new List<string>();
//        filesToProcess.Add(Path.Combine(mergePath,"merge1.txt"));
//        filesToProcess.Add(Path.Combine(mergePath,"merge2.txt"));
//        filesToProcess.Add(Path.Combine(mergePath,"merge3.txt"));
//        filesToProcess.Add(Path.Combine(mergePath,"merge4.txt"));

//        List<ProcessingStreamToMerge> list = new List<ProcessingStreamToMerge>();
//        list.Add(new  ProcessingStreamToMerge(1,filesToProcess[0]));
//        list.Add(new  ProcessingStreamToMerge(2,filesToProcess[1]));
//        list.Add(new  ProcessingStreamToMerge(3,filesToProcess[2]));
//        list.Add(new  ProcessingStreamToMerge(4,filesToProcess[3]));

//        for (int i = list.Count-1; i > -1; i--)
//        {
//            var elementRead = list[i].LoadNextEntry();
//            if (!elementRead)
//            {
//                list.RemoveAt(i);
//            }
//        }

//        using (StreamWriter outputFile = new StreamWriter(Path.Combine(mergePath,
//                   $"bigfile_sorted_ALL_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
//        {
//            while (list.Any())
//            {
//                var item =  list.OrderBy(x => x.LastEntry.Name).ThenBy(x => x.LastEntry.Number).First();

//               outputFile.Write(item.LastEntry.Number);
//               outputFile.Write(".");
//               outputFile.WriteLine(item.LastEntry.Name);

//               var elementRead = item.LoadNextEntry();
//               if (!elementRead)
//               {
//                   list.RemoveAll(x => x.Id == item.Id);
//               }
//            }
//        }
//    }

//    public class ProcessingStreamToMerge
//    {
//        private StreamReader _streamReader = null;
//        private int position;
//        private int counter = 0;
//        string  line = String.Empty;
//        private UnitTest1.BigDataEntry lastEntry;
//        private int _id = 0;

//        public ProcessingStreamToMerge(int id, string fileName)
//        {
//            _streamReader = new StreamReader(fileName);
//            _id = id;
//        }

//        public int Id
//        {
//            get => _id;
//        }

//        public UnitTest1.BigDataEntry LastEntry
//        {
//            get => lastEntry;
//            set => lastEntry = value;
//        }

//        public bool LoadNextEntry()
//        {
//            line = _streamReader.ReadLine();
//            if (line == null) return false;
//            position = line.IndexOf(".");
//            lastEntry.Number = long.Parse(line.Substring(0, position));
//            lastEntry.Name = line.Substring(position + 1);
//            return true;
//        }
//    }

//}