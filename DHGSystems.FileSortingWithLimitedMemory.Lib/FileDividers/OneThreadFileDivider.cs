using DHGSystems.FileSortingWithLimitedMemory.Common.Logging;
using DHGSystems.FileSortingWithLimitedMemory.Lib.Model;
using System.Diagnostics;

namespace DHGSystems.FileSortingWithLimitedMemory.Lib.FileDividers
{
    /// <summary>
    /// Divide file into smaller files and sort this files
    /// </summary>
    public class OneThreadFileDivider : IFileDividerWithSort
    {
        private readonly string _tempPathForSortedFiles;
        private readonly string _generatedFilePrefix;
        private readonly IDhgSystemsLogger _logger;
        private readonly long _maxLinesPerFile;

        public OneThreadFileDivider(string tempPath, long maxLinesPerFile, string generatedFilePrefix, IDhgSystemsLogger logger)
        {
            this._tempPathForSortedFiles = tempPath;
            this._maxLinesPerFile = maxLinesPerFile;
            this._generatedFilePrefix = generatedFilePrefix;
            this._logger = logger;
        }

        public IEnumerable<string> DivideFileWithSort(string fileToDived, long maxLinesBeforeSort)
        {
            List<string> generatedFiles = new List<string>();
            var watch = Stopwatch.StartNew();
            Process proc = Process.GetCurrentProcess();
            _logger.Info("OneThreadFileDivider", $"Starting dividing file {fileToDived} ");

            using (StreamReader sr = File.OpenText(fileToDived))
            {
                int position;
                int lineCount = 0;
                int fileNumber = 1;
                string[] allStrings = new string[_maxLinesPerFile];
                string lineText = String.Empty;
                BigDataEntryRef[] loadedValues = new BigDataEntryRef[_maxLinesPerFile];

                while ((lineText = sr.ReadLine()) != null)
                {
                    position = lineText.IndexOf(".");
                    allStrings[lineCount] = lineText.Substring(position + 1);
                    loadedValues[lineCount].Number = long.Parse(lineText.Substring(0, position));
                    loadedValues[lineCount].Name = lineCount;
                    lineCount++;

                    if (lineCount == maxLinesBeforeSort)
                    {
                        proc.Refresh();
                        var newfileName = GetFileName();
                        _logger.Info("OneThreadFileDivider", $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds.ToString()}," +
                                                             $" Memory usage {proc.PrivateMemorySize64:N1} batch of {_maxLinesPerFile} read for file nr. {fileNumber}. File will be saved as {newfileName}.");

                        using (StreamWriter outputFile = new StreamWriter(newfileName))
                        {
                            var sorted = loadedValues.AsParallel().OrderBy(x => allStrings[x.Name]).ThenBy(y => y.Number).ToArray();
                            _logger.Info("OneThreadFileDivider", $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds.ToString()}," +
                                                                 $" Memory usage {proc.PrivateMemorySize64:N1}  batch of {_maxLinesPerFile} sorted for file nr. {fileNumber} . File name {newfileName}.");
                            for (int i = 0; i < sorted.Length; i++)
                            {
                                outputFile.Write(sorted[i].Number);
                                outputFile.Write(".");
                                outputFile.WriteLine(allStrings[sorted[i].Name]);

                            }
                            generatedFiles.Add(newfileName);
                        }

                        proc.Refresh();
                        _logger.Info("OneThreadFileDivider", $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds.ToString()}," +
                                                             $" Memory usage {proc.PrivateMemorySize64:N1} File nr. {fileNumber} saved. File name {newfileName}.");
                        GC.Collect();
                        fileNumber++;
                        lineCount = 0;
                    }
                }
                proc.Refresh();

                if (lineCount > 0)
                {
                    var newfileName = GetFileName();
                    _logger.Info("OneThreadFileDivider", $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds.ToString()}," +
                                                         $" Memory usage {proc.PrivateMemorySize64:N1} Processing rest of rows {lineCount} read for file file nr. {fileNumber}. File will be saved as {newfileName}.");
                    using (StreamWriter outputFile = new StreamWriter(newfileName))
                    {
                        var sorted = loadedValues[0..lineCount].AsParallel().OrderBy(x => allStrings[x.Name]).ThenBy(y => y.Number).ToArray();
                        _logger.Info("OneThreadFileDivider", $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds.ToString()}," +
                                                             $" Memory usage {proc.PrivateMemorySize64:N1}  batch of {_maxLinesPerFile} sorted for file nr. {fileNumber} . File name {newfileName}.");
                        for (int i = 0; i < sorted.Length; i++)
                        {
                            outputFile.Write(sorted[i].Number);
                            outputFile.Write(".");
                            outputFile.WriteLine(allStrings[sorted[i].Name]);
                        }
                    }
                    generatedFiles.Add(newfileName);
                    proc.Refresh();
                    _logger.Info("OneThreadFileDivider", $"Dividing file {fileToDived}. Time {watch.ElapsedMilliseconds.ToString()}," +
                                                         $" Memory usage {proc.PrivateMemorySize64:N1} File nr. {fileNumber} saved. File name {newfileName}.");
                }
            }

            proc.Refresh();
            _logger.Info("OneThreadFileDivider", $"Dividing file {fileToDived} completed. Total time {watch.ElapsedMilliseconds.ToString()}," +
                                                 $" Memory usage {proc.PrivateMemorySize64:N1}");
            return generatedFiles;
        }

        private string GetFileName()
        {
            string newfileName = Path.Combine(_tempPathForSortedFiles,
                $"{_generatedFilePrefix}_{DateTime.Now.ToString("yyyyMMdd_hhmmssfff")}.txt");
            return newfileName;
        }
    }
}
