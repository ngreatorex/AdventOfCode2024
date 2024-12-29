namespace Day08;

internal static class MultiDimensionalArrayExtensions
{
    internal static IEnumerable<T> ToEnumerable<T>(this Array target)
    {
        foreach (var item in target)
            yield return (T)item;
    }

    internal static IEnumerable<(T a, T b)> Pairs<T>(this IEnumerable<T> elements)
    {
        return elements.SelectMany((e, i) => elements.Skip(i + 1).Select(c => (e, c)));
    }
}
