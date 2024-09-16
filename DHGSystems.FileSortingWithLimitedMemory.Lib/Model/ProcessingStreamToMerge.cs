namespace DHGSystems.FileSortingWithLimitedMemory.Lib.Model
{
    public class ProcessingStreamToMerge
    {
        private StreamReader _streamReader = null;
        private int position;
        private int counter = 0;
        private string line = String.Empty;
        private BigDataEntry lastEntry;
        private int _id = 0;

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
            position = line.IndexOf(".");
            lastEntry.Number = long.Parse(line.Substring(0, position));
            lastEntry.Name = line.Substring(position + 1);
            return true;
        }
    }
}