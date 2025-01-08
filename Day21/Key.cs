namespace Day21;

public class Key(char value, int y, int x)
{
    public char Value { get; } = value;
    public int Y { get; } = y;
    public int X { get; } = x;

    public override string ToString() => $"{{ Key '{Value}' }}";
}
