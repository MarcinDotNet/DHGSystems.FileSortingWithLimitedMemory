namespace DHGSystems.FileSortingWithLimitedMemory.Lib.Model;

public struct BigDataEntryWithFileId : IEquatable<BigDataEntryWithFileId>, IComparable<BigDataEntryWithFileId>
{
    public int FileId { get; set; }

    public long Number { get; set; }
    public string Name { get; set; }

    public bool Equals(BigDataEntryWithFileId other)
    {
        return other.Name == Name && other.Number == Number;
    }

    public int CompareTo(BigDataEntryWithFileId other)
    {
        var nameCompare = System.String.Compare(Name, other.Name, StringComparison.Ordinal);
        return nameCompare != 0 ? nameCompare : Number.CompareTo(other.Number);
    }
}