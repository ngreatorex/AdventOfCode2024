namespace Day20;

public record Cell(int Y, int X)
{
    public bool IsStart { get; init; }
    public bool IsEnd { get; init; }
    public bool ContainsWall { get; init; }

    public char ToChar() => ContainsWall ? '#' : IsEnd ? 'E' : IsStart ? 'S' : '.';

    public override string ToString() => $"{{Cell [{Y}, {X}]}}";
}