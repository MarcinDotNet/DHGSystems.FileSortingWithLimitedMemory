namespace DHGSystems.FileSortingWithLimitedMemory.Lib.Model
{
    public class ProcessingStreamToMerge
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

        public bool LoadNextEntry()
        {
            line = _streamReader.ReadLine();
            if (line == null) return false;
            position = line.IndexOf(".", StringComparison.Ordinal);
            lastEntry.Number = long.Parse(line.Substring(0, position));
            lastEntry.Name = line.Substring(position + 1);
            return true;
        }
    }
}