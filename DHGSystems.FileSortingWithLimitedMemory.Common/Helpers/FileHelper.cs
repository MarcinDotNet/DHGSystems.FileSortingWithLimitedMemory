namespace DHGSystems.FileSortingWithLimitedMemory.Common.Helpers
{
    public static class FileHelper
    {
        public static string GetLineContentFromFile(string fileName, long lineNumber)
        {
            using (StreamReader sr = File.OpenText(fileName))
            {
                string lineText = String.Empty;
                long lineCount = 0;
                while ((lineText = sr.ReadLine()) != null)
                {
                    lineCount++;
                    if (lineCount == lineNumber)
                    {
                        return lineText;
                    }
                }
            }
            return String.Empty;
        }

        public static long GetFileSizeInMB(string fileName)
        {
            return new FileInfo(fileName).Length / 1024 / 1024;
        }
    }
}