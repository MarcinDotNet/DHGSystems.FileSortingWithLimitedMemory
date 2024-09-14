public struct BigDataEntry : IEquatable<BigDataEntry>, IComparable<BigDataEntry>
{
    public long Number { get; set; }

    public string Name { get; set; }

    public bool Equals(BigDataEntry other)
    {
        return other.Name == Name && other.Number == Number;
    }

    public int CompareTo(BigDataEntry other)
    {
        var nameCompare = Name.CompareTo(other.Name);
        return nameCompare != 0 ? nameCompare : Number.CompareTo(other.Number);
    }
}