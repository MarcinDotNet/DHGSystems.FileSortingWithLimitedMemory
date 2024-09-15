using DHGSystems.FileSortingWithLimitedMemory.Lib.Model;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort
{
    public class SimpleFileMergerWithSorting : IFileMergerWithSorting
    {
        public void MergeFilesWithSort(string[] filesToMerge, string outputFilePath)
        {
            List<ProcessingStreamToMerge> list = new List<ProcessingStreamToMerge>();
            for (int i = 0; i < filesToMerge.Length; i++)
            {
                list.Add(new ProcessingStreamToMerge(i, filesToMerge[i]));
            }

            bool elementRead;

            //Load first element from each file
            for (int i = list.Count - 1; i > -1; i--)
            {
                elementRead = list[i].LoadNextEntry();
                if (!elementRead)
                {
                    list.RemoveAt(i);
                }
            }

            using (StreamWriter outputFile = new StreamWriter(outputFilePath))
            {
                ProcessingStreamToMerge item;
                bool firstLine = true; 
                while (list.Any())
                {
                    // to not set new line at the beginning of the file and to not set new line at the end of the file
                    if (!firstLine)
                    {
                        outputFile.WriteLine();
                    }
                    else
                    {
                        firstLine = false;
                    }

                    item = list.OrderBy(x => x.LastEntry.Name).ThenBy(x => x.LastEntry.Number).First();

                    outputFile.Write(item.LastEntry.Number);
                    outputFile.Write(".");
                    outputFile.Write(item.LastEntry.Name);

                    elementRead = item.LoadNextEntry();
                    if (!elementRead)
                    {
                        list.RemoveAll(x => x.Id == item.Id);
                    }
                }
            }
        }
    }
}
