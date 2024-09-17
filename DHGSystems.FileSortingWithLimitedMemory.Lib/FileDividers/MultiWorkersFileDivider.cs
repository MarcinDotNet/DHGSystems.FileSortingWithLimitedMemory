using DHGSystems.FileSortingWithLimitedMemory.Common.Helpers;
using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Model;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers
{
    /// <summary>
    /// Divide file into smaller files and sort this files
    /// </summary>
    public class MultiWorkersFileDivider : IFileDividerWithSort
    {
        private readonly string tempPathForSortedFiles;
        private readonly string _generatedFilePrefix;
        private readonly IDhgSystemsLogger _logger;
        private const string ClassName = "MultiThreadFileDivider";
        private const int MaxNumberOfSortWorkers = 4;

        public MultiWorkersFileDivider(string tempPath, string generatedFilePrefix, IDhgSystemsLogger logger)
        {
            this.tempPathForSortedFiles = tempPath;
            this._generatedFilePrefix = generatedFilePrefix;
            this._logger = logger;
        }

        public void DivideFileWithSort(string fileToDived, long maxLinesBeforeSort, ConcurrentQueue<string> filesProcessed)
        {
            var watch = Stopwatch.StartNew();
            _logger.Info(ClassName, $"Starting dividing file {fileToDived} ");

            decimal totalRows = 0;
            Task[] tasks = new Task[MaxNumberOfSortWorkers];
            ConcurrentQueue<string> generatedFilesQueue = new ConcurrentQueue<string>();
            using (StreamReader sr = File.OpenText(fileToDived))
            {
                int position;
                int lineCount = 0;
                int fileNumber = 1;
                string lineText = String.Empty;
                int taskNumber = 0;
                List<string[]> allStrings = new List<string[]>();
                List<BigDataEntryRef[]> loadedValues = new List<BigDataEntryRef[]>();

                //initialize memory
                for (int i = 0; i < MaxNumberOfSortWorkers; i++)
                {
                    allStrings.Add(new string[maxLinesBeforeSort]);
                    loadedValues.Add(new BigDataEntryRef[maxLinesBeforeSort]);
                }
                var currentStringArray = allStrings[taskNumber];
                var currentLoadedValues = loadedValues[taskNumber];

                List<(int, int)> listOfTasksParameters = new List<(int, int)>();
                while ((lineText = sr.ReadLine()) != null)
                {
                    position = lineText.IndexOf(".", StringComparison.CurrentCulture);
                    currentStringArray[lineCount] = lineText.Substring(position + 1);
                    currentLoadedValues[lineCount].Number = long.Parse(lineText.Substring(0, position));
                    currentLoadedValues[lineCount].Name = lineCount;
                    lineCount++;

                    // do job for one set
                    if (lineCount == maxLinesBeforeSort)
                    {
                        totalRows += lineCount;
                        listOfTasksParameters.Add((taskNumber, fileNumber));
                        int lastElement = listOfTasksParameters.Count - 1;
                        tasks[taskNumber] = Task.Run(() => ProcessFile(listOfTasksParameters[lastElement].Item1, fileToDived, maxLinesBeforeSort,
                            watch, listOfTasksParameters[lastElement].Item2,
                            loadedValues[listOfTasksParameters[lastElement].Item1], allStrings[listOfTasksParameters[lastElement].Item1], generatedFilesQueue));
                        fileNumber++;
                        taskNumber++;
                        lineCount = 0;
                        if (taskNumber == MaxNumberOfSortWorkers)
                        {
                            taskNumber = 0;
                        }
                        if (tasks[taskNumber] != null)
                        {
                            tasks[taskNumber].Wait();
                        }
                        currentStringArray = allStrings[taskNumber];
                        currentLoadedValues = loadedValues[taskNumber];
                        GC.Collect();
                        var completedFile = string.Empty;
                        while (generatedFilesQueue.TryDequeue(out completedFile))
                        {
                            filesProcessed.Enqueue(completedFile);
                        }
                    }
                }
                // do job for last step
                if (lineCount > 0)
                {
                    var newfileName = GetFileName(fileNumber);
                    _logger.Info(ClassName, $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds:N1} ms," +
                                            $" Memory usage {ProcessHelper.GetUsedMemoryInMb()} MB Processing rest of rows {lineCount} read for file file nr. {fileNumber}. File will be saved as {newfileName}.");
                    using (StreamWriter outputFile = new StreamWriter(newfileName))
                    {
                        outputFile.AutoFlush = false;
                        var sorted = currentLoadedValues[0..lineCount].AsParallel().OrderBy(x => currentStringArray[x.Name]).ThenBy(y => y.Number).ToArray();
                        _logger.Info(ClassName, $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds:N1} ms," +
                                                             $" Memory usage {ProcessHelper.GetUsedMemoryInMb()} MB  batch of {lineCount} sorted for file nr. {fileNumber} . File name {newfileName}.");
                        int sortedLength = sorted.Length;
                        int lastLine = sorted.Length - 1;
                        for (int i = 0; i < sortedLength; i++)
                        {
                            outputFile.Write(sorted[i].Number);
                            outputFile.Write(".");
                            if (i == lastLine)
                            {
                                outputFile.Write(currentStringArray[sorted[i].Name]);
                            }
                            else
                            {
                                outputFile.WriteLine(currentStringArray[sorted[i].Name]);
                            }
                        }
                        outputFile.Flush();
                    }

                    generatedFilesQueue.Enqueue(newfileName);

                    _logger.Info(ClassName, $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds:N1} ms," +
                                            $" Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB File nr. {fileNumber} saved. File name {newfileName}.");
                }
                totalRows += lineCount;
            }
            _logger.Info(ClassName, $"Dividing file {fileToDived} completed. Total time {watch.ElapsedMilliseconds:N1} ms," +
                                    $" Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB. Total lines in file {totalRows} ");

            // wait for all task to complete
            foreach (var task in tasks)
            {
                if (task != null)
                {
                    task.Wait();
                }
            }

            // read rest of files
            var responseFile = string.Empty;
            while (generatedFilesQueue.TryDequeue(out responseFile))
            {
                filesProcessed.Enqueue(responseFile);
            }
        }

        private void ProcessFile(int taskId, string fileToDived, long maxLinesBeforeSort, Stopwatch watch, int fileNumber,
            BigDataEntryRef[] loadedValues, string[] allStrings, ConcurrentQueue<string> generatedFilesQueue)
        {
            var newfileName = GetFileName(fileNumber);
            _logger.Info(ClassName, $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds:N1} ms," +
                                    $" Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB batch of {maxLinesBeforeSort} read for file nr. {fileNumber}. Task {taskId}. File will be saved as {newfileName}.");

            using (StreamWriter outputFile = new StreamWriter(newfileName))
            {
                outputFile.AutoFlush = false;
                var sorted = loadedValues.AsParallel().OrderBy(x => allStrings[x.Name]).ThenBy(y => y.Number).ToArray();
                _logger.Info(ClassName, $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds:N1} ms," +
                                        $" Memory usage {ProcessHelper.GetUsedMemoryInMb()} MB  batch of {maxLinesBeforeSort} sorted for file nr. {fileNumber}.Task {taskId}. File name {newfileName}.");
                int lastLine = sorted.Length - 1;

                for (int i = 0; i < sorted.Length; i++)
                {
                    outputFile.Write(sorted[i].Number);
                    outputFile.Write(".");
                    if (i == lastLine)
                    {
                        outputFile.Write(allStrings[sorted[i].Name]);
                    }
                    else
                    {
                        outputFile.WriteLine(allStrings[sorted[i].Name]);
                    }
                }
                outputFile.Flush();
            }

            _logger.Info(ClassName, $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds:N1} ms," +
                                    $" Memory usage {ProcessHelper.GetUsedMemoryInMb()} MB File nr. {fileNumber} saved.Task {taskId}. File name {newfileName}.");

            generatedFilesQueue.Enqueue(newfileName);
        }

        private string GetFileName(int fileNumber)
        {
            string newfileName = Path.Combine(tempPathForSortedFiles,
                $"{_generatedFilePrefix}_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}_{fileNumber:D5}.txt");
            return newfileName;
        }
    }
}