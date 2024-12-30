namespace Day11;

public interface IStoneStorage : IEnumerable<(long stone, long count)>
{
    public long Count { get; }
    public long DistinctStones { get; }
    public void Add(long item, long count = 1);
}
