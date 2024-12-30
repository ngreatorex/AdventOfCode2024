namespace Day11;

internal class StoneStorageFactory
{
    internal static IStoneStorage List() => new ListBackedStoneStorage();
    internal static IStoneStorage Dictionary() => new DictionaryBackedStoneStorage();
}
