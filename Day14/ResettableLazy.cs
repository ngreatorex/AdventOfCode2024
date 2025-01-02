using System.Diagnostics.CodeAnalysis;

namespace Day14;

internal class ResettableLazy<T>
{
    protected Func<T> _factory;
    protected Lazy<T> _lazy;

    internal ResettableLazy(Func<T> factory)
    {
        _factory = factory;
        Reset();
    }

    internal T Value => _lazy.Value;
    internal bool IsValueCreated => _lazy.IsValueCreated;

    internal void Reset()
    {
        _lazy = new Lazy<T>(_factory);
    }
}
