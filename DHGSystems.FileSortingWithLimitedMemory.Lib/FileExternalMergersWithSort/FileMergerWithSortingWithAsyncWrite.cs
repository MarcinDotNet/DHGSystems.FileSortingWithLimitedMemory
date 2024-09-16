using DHGSystems.FileSortingWithLimitedMemory.Lib.Model;
using System.Collections.Concurrent;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort
{
    public class FileMergerWithSortingWithAsyncWrite : IFileMergerWithSorting
    {
        private ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private bool _isWriting = true;

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

            Task ascyWriteTask = Task.Run(() => WriteToFile(outputFilePath));

            ProcessingStreamToMerge item;
            while (list.Any())
            {
                item = list.OrderBy(x => x.LastEntry.Name).ThenBy(x => x.LastEntry.Number).First();
                _queue.Enqueue(item.LastEntry.Name);
                elementRead = item.LoadNextEntry();
                if (!elementRead)
                {
                    list.RemoveAll(x => x.Id == item.Id);
                }
            }
            // locking not needed because we just change flag once
            this._isWriting = false;

            ascyWriteTask.Wait();
        }

        public void WriteToFile(string outputFilePath)
        {
            bool firstLine = true;

            using (StreamWriter outputFile = new StreamWriter(outputFilePath))
            {
                outputFile.AutoFlush = false;
                int flushCount = 0;
                string item;
                while (_isWriting)
                {
                    if (_queue.TryDequeue(out item))
                    {
                        // to not set new line at the beginning of the file and to not set new line at the end of the file
                        if (!firstLine)
                        {
                            outputFile.Write(Environment.NewLine);
                        }
                        else
                        {
                            firstLine = false;
                        }
                        outputFile.Write(item);
                        flushCount++;
                        if (flushCount == 50000)
                        {
                            outputFile.Flush();
                            flushCount = 0;
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                //read rest of the queue
                while (_queue.TryDequeue(out item))
                {
                    if (!firstLine)
                    {
                        outputFile.Write(Environment.NewLine);
                    }
                    else
                    {
                        firstLine = false;
                    }
                    outputFile.Write(item);
                    flushCount++;
                    if (flushCount == 50000)
                    {
                        outputFile.Flush();
                        flushCount = 0;
                    }
                }
                outputFile.Flush();
            }
        }
    }
}