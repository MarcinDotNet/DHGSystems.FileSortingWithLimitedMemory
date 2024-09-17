using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Model;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort
{
    public class FileMergerWithSortingCumulated : IFileMergerWithSorting
    {
        private readonly IDhgSystemsLogger _logger;
        private const string ClassName = "FileMergerWithSortingCumulated";

        public FileMergerWithSortingCumulated(IDhgSystemsLogger logger)
        {
            this._logger = logger;
        }

        public void MergeFilesWithSort(string[] filesToMerge, string outputFilePath, bool deleteFile = true)
        {
            int fileToProcess = filesToMerge.Length;
            string lastLine = String.Empty;
            _logger.Info(ClassName, $"Run for output file: {outputFilePath}. Files to process {fileToProcess}");
            // open files
            StreamReader[] streamReaders = new StreamReader[fileToProcess];
            for (int i = 0; i < filesToMerge.Length; i++)
            {
                streamReaders[i] = new StreamReader(filesToMerge[i]);
            }

            int position;
            string lineText = String.Empty;
            BigDataEntryWithFileId[] entries = new BigDataEntryWithFileId[fileToProcess];
            for (int i = fileToProcess - 1; i > -1; i--)
            {
                lineText = streamReaders[i].ReadLine();
                if (lineText == null)
                {
                    streamReaders[i].Close();
                    streamReaders[i].Dispose();
                    var entriesList = entries.ToList();
                    entriesList.RemoveAt(i);
                    entries = entriesList.ToArray();
                    _logger.Info(ClassName, $"Run for output file: {outputFilePath}. Files fully read {filesToMerge[i]}");
                    continue;
                }

                position = lineText.IndexOf(".", StringComparison.CurrentCulture);
                entries[i].Number = long.Parse(lineText.Substring(0, position));
                entries[i].Name = lineText.Substring(position + 1);
                entries[i].FileId = i;
            }

            using (StreamWriter outputFile = new StreamWriter(outputFilePath))
            {
                outputFile.AutoFlush = false;
                bool firstLine = true;
                int flushCount = 0;

                while (fileToProcess > 0)
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
                    Array.Sort(entries);

                    outputFile.Write(entries[0].Number);
                    outputFile.Write(".");
                    outputFile.Write(entries[0].Name);

                    lineText = streamReaders[entries[0].FileId].ReadLine();

                    if (lineText == null)
                    {
                        streamReaders[entries[0].FileId].Close();
                        streamReaders[entries[0].FileId].Dispose();
                        fileToProcess--;
                        if (fileToProcess == 0)
                        {
                            lastLine = entries[0].Number + "." + entries[0].Name;
                        }
                        _logger.Info(ClassName, $"Run for output file: {outputFilePath}. Files fully read {filesToMerge[entries[0].FileId]}");
                        var entriesList = entries.ToList();
                        entriesList.RemoveAt(0);
                        entries = entriesList.ToArray();

                        continue;
                    }
                    position = lineText.IndexOf(".", StringComparison.CurrentCulture);
                    entries[0].Number = long.Parse(lineText.Substring(0, position));
                    entries[0].Name = lineText.Substring(position + 1);

                    flushCount++;
                    if (flushCount == 5000)
                    {
                        outputFile.Flush();
                        flushCount = 0;
                    }
                }
                outputFile.Flush();
            }

            if (deleteFile)
            {
                foreach (var file in filesToMerge)
                {
                    if (File.Exists(file))
                    {
                        _logger.Info(ClassName, $"Run for output file: {outputFilePath} Deleting file : {file}");
                        File.Delete(file);
                    }
                }
            }
            _logger.Info(ClassName, $"Run for output file: {outputFilePath}. File saved. Last line {lastLine}");
        }
    }
}