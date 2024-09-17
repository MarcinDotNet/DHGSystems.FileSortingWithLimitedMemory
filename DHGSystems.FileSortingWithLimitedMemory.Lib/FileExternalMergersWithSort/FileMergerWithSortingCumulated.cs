using DHGSystems.FileSortingWithLimitedMemory.Lib.Model;
using System.Linq;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort
{
    public class FileMergerWithSortingCumulated : IFileMergerWithSorting
    {
        public void MergeFilesWithSort(string[] filesToMerge, string outputFilePath)
        {
            int startFileCount = filesToMerge.Length;
            
            // open files 
            StreamReader[] streamReaders = new StreamReader[startFileCount];
            for (int i = 0; i < filesToMerge.Length; i++)
            {
               streamReaders[i] = new StreamReader(filesToMerge[i]);
            }

            for (int i = 0; i < startFileCount; i++)
            {
                
            }
            
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
                outputFile.AutoFlush = false;
                ProcessingStreamToMerge item;
                bool firstLine = true;
                int flushCount = 0;
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
                    flushCount++;
                    if (flushCount == 5000)
                    {
                        outputFile.Flush();
                        flushCount = 0;
                    }
                }
                outputFile.Flush();
            }

            foreach(var file in filesToMerge)
            {
                File.Delete(file);
            }
        }
    }
}