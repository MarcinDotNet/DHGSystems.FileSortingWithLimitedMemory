using DHGSystems.FileSortingWithLimitedMemory.Lib.Model;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort
{
    public class SimpleFileMergerWithSorting : IFileMergerWithSorting
    {

        private string _mergePath;
        public void MergeFilesWithSort(string[] filesToMerge, string outputFilePath)
        {
            List<ProcessingStreamToMerge> list = new List<ProcessingStreamToMerge>();
            for (int i = 0; i < filesToMerge.Length; i++)
            {
                list.Add(new ProcessingStreamToMerge(i, filesToMerge[i]));
            }

            for (int i = list.Count - 1; i > -1; i--)
            {
                var elementRead = list[i].LoadNextEntry();
                if (!elementRead)
                {
                    list.RemoveAt(i);
                }
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(_mergePath,
                       $"bigfile_sorted_ALL_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt")))
            {
                while (list.Any())
                {
                    var item = list.OrderBy(x => x.LastEntry.Name).ThenBy(x => x.LastEntry.Number).First();

                    outputFile.Write(item.LastEntry.Number);
                    outputFile.Write(".");
                    outputFile.WriteLine(item.LastEntry.Name);

                    var elementRead = item.LoadNextEntry();
                    if (!elementRead)
                    {
                        list.RemoveAll(x => x.Id == item.Id);
                    }
                }
            }
        }
    }
}
