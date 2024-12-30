using System.Collections;

namespace Day11;

internal class DictionaryBackedStoneStorage : IStoneStorage
{
    private readonly Dictionary<long, long> storage = [];

    public long Count => storage.Values.Sum();

    public long DistinctStones => storage.LongCount(kvp => kvp.Value != 0);

    public static bool IsReadOnly => false;

    public void Add(long item, long count = 1)
    {
        if (!storage.ContainsKey(item))
            storage[item] = 0;
        storage[item] += count;
    }

    public IEnumerator<(long stone, long count)> GetEnumerator() => storage.Where(kvp => kvp.Value != 0).Select(kvp => (kvp.Key, kvp.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public override string ToString() => "Stone histogram:\n" + string.Join("\n", storage.OrderBy(kvp => kvp.Key).Select(kvp => $"\t{kvp.Key}: {kvp.Value}"));
}
