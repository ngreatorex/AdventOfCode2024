using System.Diagnostics;

namespace Day16;

[DebuggerDisplay(@"\{Cell [{Y}, {X}]\}")]
public record Cell(int Y, int X)
{
    public bool IsStart { get; init; }
    public bool IsEnd { get; init; }
    public bool ContainsWall { get; init; }

    public override string ToString() => ContainsWall ? "#" : IsEnd ? "E" : IsStart ? "S" : ".";
}