using System.Collections;

namespace Day11;

public class ListBackedStoneStorage : IStoneStorage
{
    private readonly List<long> storage = [];

    public long Count => storage.Count;

    public long DistinctStones => storage.GroupBy(l => l).LongCount();

    public void Add(long item, long count = 1)
    {
        for (long i = 0; i < count; i++)
            storage.Add(item);
    }

    public IEnumerator<(long stone, long count)> GetEnumerator() => storage.GroupBy(l => l).Select(g => (g.Key, g.LongCount())).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => string.Join(" ", storage);
}
