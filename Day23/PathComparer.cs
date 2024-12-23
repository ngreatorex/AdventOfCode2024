namespace Day23;

public class PathComparer : IEqualityComparer<List<Computer>>
{
    public bool Equals(List<Computer>? x, List<Computer>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.SequenceEqual(y);
    }

    public int GetHashCode(List<Computer> obj) => string.Join('|', obj.Select(c => c.Identifier)).GetHashCode();
}

public class NodeSetComparer : IEqualityComparer<HashSet<Computer>>
{
    public bool Equals(HashSet<Computer>? x, HashSet<Computer>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.SetEquals(y);
    }

    public int GetHashCode(HashSet<Computer> obj) => string.Join('|', new HashSet<string>(obj.Select(c => c.Identifier))).GetHashCode();
}
