namespace Day23;

public record Computer(string Identifier) : IComparable<Computer>
{
    public int CompareTo(Computer? other)
    {
        return ReferenceEquals(null, other)
            ? 1
            : string.Compare(Identifier, other.Identifier, StringComparison.Ordinal);
    }

    public List<Computer> ToList() => [this];

    public override string ToString() => Identifier;
}

