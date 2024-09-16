using DHGSystems.FileSortingWithLimitedMemory.Common.Helpers;
using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Configuration;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers;
using DHGSystems.FileSortingWithLimitedMemory.Lib.FileExternalMergersWithSort;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.Controllers
{
    public class FileSortingController
    {
        private readonly FileSortingAppConfiguration _configuration;
        private readonly IDhgSystemsLogger _logger;
        private const string ClassName = "FileSortingController";
        private readonly IFileDividerWithSort _fileSorterDividerWithSort;
        private readonly IFileMergerWithSorting _fileMergerWithSorting;
        public const int startMergeFileCount = 5;

        public FileSortingController(FileSortingAppConfiguration configuration,
            IDhgSystemsLogger logger, IFileDividerWithSort fileSorterDividerWithSort, IFileMergerWithSorting fileMergerWithSorting)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._fileSorterDividerWithSort = fileSorterDividerWithSort;
            this._fileMergerWithSorting = fileMergerWithSorting;
        }

        public void SortFile(string inputFileFullFileName, string outputFileFullFileName)
        {
            _logger.Info(ClassName, $"Start sorting file {inputFileFullFileName}, file will be saved as {outputFileFullFileName}");
            if (inputFileFullFileName == null) { throw new ArgumentNullException(nameof(inputFileFullFileName)); }
            if (outputFileFullFileName == null) { throw new ArgumentNullException(nameof(outputFileFullFileName)); }
            if (!File.Exists(inputFileFullFileName)) { throw new ArgumentException("Input file does not exist"); }
            if (!Directory.Exists(_configuration.TempFolderPath))
            {
                Directory.CreateDirectory(_configuration.TempFolderPath);
            }

            decimal fileSizesInMB = new FileInfo(inputFileFullFileName).Length / 1024 / 1024;

            _logger.Info(ClassName, $"File size {fileSizesInMB} MB.");

            var totalWatch = Stopwatch.StartNew();
            _logger.Info(ClassName, $"Start dividing file into sorted files. Temp folder: {_configuration.TempFolderPath}. Max lines in one file: {_configuration.MaxLinesBeforeSort}");
            List<string> filesToProcess = new List<string>();

            ConcurrentQueue<string> filesProcessedQueue = new ConcurrentQueue<string>();
            bool firstFile = true;
            var sortingTask = Task.Run(
                () => this._fileSorterDividerWithSort.DivideFileWithSort(inputFileFullFileName,
                _configuration.MaxLinesBeforeSort, filesProcessedQueue));
            Stopwatch mergeWatch = null;
            int mergeFileTaskId = 0;
            int fileCount = 0;
            List<Task> mergeTasks = new List<Task>();
            List<(string, string[])> listOfMergeTasksParameters = new List<(string, string[])>();

            while (!sortingTask.IsCompleted)
            {
                if (filesProcessedQueue.TryDequeue(out string fileName))
                {
                    filesToProcess.Add(fileName);
                    if (firstFile)
                    {
                        firstFile = false;
                        _logger.Info(ClassName, $" First file created: {fileName}. File size {FileHelper.GetFileSizeInMb(fileName)} MB.");
                    }

                    fileCount++;
                    if (filesToProcess.Count == startMergeFileCount)
                    {
                        int currentMergeFileTaskId = mergeFileTaskId;
                        listOfMergeTasksParameters.Add((GetMergeFileName(currentMergeFileTaskId), filesToProcess.ToArray()));
                        _logger.Info(ClassName, $"Start merging {startMergeFileCount} files into {listOfMergeTasksParameters[mergeFileTaskId].Item1} . Time {totalWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");
                        mergeTasks.Add(Task.Run(() =>
                        {
                            string fileName = listOfMergeTasksParameters[currentMergeFileTaskId].Item1;
                            this._fileMergerWithSorting.MergeFilesWithSort(
                                    listOfMergeTasksParameters[currentMergeFileTaskId].Item2,
                                    listOfMergeTasksParameters[currentMergeFileTaskId].Item1);
                            filesProcessedQueue.Enqueue(fileName);
                        }));
                        mergeFileTaskId++;
                        filesToProcess.Clear();
                        mergeWatch = Stopwatch.StartNew();
                    }
                }
                Thread.Sleep(100);
            }

            _logger.Info(ClassName, $"Dividing file into sorted files finished. Time {totalWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");

            while (filesProcessedQueue.TryDequeue(out string fileName))
            {
                filesToProcess.Add(fileName);
                if (firstFile)
                {   
                    firstFile = false;
                    _logger.Info(ClassName, $" First file created: {fileName}. File size {FileHelper.GetFileSizeInMb(fileName)} MB.");
                }

                fileCount++;
                if (filesToProcess.Count == startMergeFileCount)
                {
                    int currentMergeFileTaskId = mergeFileTaskId;
                    listOfMergeTasksParameters.Add((GetMergeFileName(currentMergeFileTaskId), filesToProcess.ToArray()));
                    mergeTasks.Add(Task.Run(() =>
                    {
                        string fileName = listOfMergeTasksParameters[currentMergeFileTaskId].Item1;
                        this._fileMergerWithSorting.MergeFilesWithSort(
                            listOfMergeTasksParameters[currentMergeFileTaskId].Item2,
                            listOfMergeTasksParameters[currentMergeFileTaskId].Item1);
                        filesProcessedQueue.Enqueue(fileName);
                    }));

                    mergeFileTaskId++;
                    filesToProcess.Clear();
                    if (mergeWatch == null)
                    {
                        mergeWatch = Stopwatch.StartNew();
                    }
                }
            }

            _logger.Info(ClassName, $"Dequeuing finished. Time {totalWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");

            foreach (var task in mergeTasks)
            {
                if (task != null)
                {
                    task.Wait();
                }
            }

            _logger.Info(ClassName, $"Initial files merging finished. Time {mergeWatch?.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");

            if (mergeWatch == null)
            {
                mergeWatch = Stopwatch.StartNew();
            }
            var restOfFiles = filesToProcess.ToList();

           var filesAlreadyMerged = listOfMergeTasksParameters.SelectMany(x => x.Item2).ToList();
            restOfFiles.AddRange(listOfMergeTasksParameters.Select(x => x.Item1).Where(y=> !filesAlreadyMerged.Any(z=>z==y)));

            _logger.Info(ClassName, $"Start final merging files into {outputFileFullFileName} . Time {totalWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB. Files to process: {restOfFiles.Count}");

            this._fileMergerWithSorting.MergeFilesWithSort(restOfFiles.ToArray(), outputFileFullFileName);

            _logger.Info(ClassName, $"Merging files finished. Time {mergeWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");

            _logger.Info(ClassName, $"Sorting file {inputFileFullFileName} finished. Time {totalWatch.ElapsedMilliseconds:N1} ms. Memory usage {ProcessHelper.GetUsedMemoryInMb():N1} MB");
        }

        private string GetMergeFileName(int fileNumber)
        {
            string newfileName = Path.Combine(_configuration.TempFolderPath,
                $"{_configuration.TempFolderPath}_merge_file_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}_{fileNumber:D5}.txt");
            return newfileName;
        }
    }
}