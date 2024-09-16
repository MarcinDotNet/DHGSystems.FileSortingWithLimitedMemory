namespace DHGSystems.FileSortingWithLimitedMemory.Lib.Model
{
    public class ProcessingStreamToMerge : IDisposable
    {
        private StreamReader? _streamReader;
        private int position;
        private string line = String.Empty;
        private BigDataEntry lastEntry;
        private int _id;

        public ProcessingStreamToMerge(int id, string fileName)
        {
            _streamReader = new StreamReader(fileName);
            _id = id;
        }

        public int Id
        {
            get => _id;
        }

        public BigDataEntry LastEntry
        {
            get => lastEntry;
            set => lastEntry = value;
        }

        public void Dispose()
        {
            if (_streamReader != null)
            {
                _streamReader.Close();
                _streamReader.Dispose();
                _streamReader = null;
            }
        }

        public bool LoadNextEntry()
        {
            line = _streamReader.ReadLine();
            if (line == null)
            {
                _streamReader.Close();
                return false;
            }

            position = line.IndexOf(".", StringComparison.Ordinal);
            lastEntry.Number = long.Parse(line.Substring(0, position));
            lastEntry.Name = line.Substring(position + 1);
            return true;
        }
    }
}