namespace Day18;

public record Coordinate
{
    public bool IsCorrupted { get; set; } = false;
    public bool IsOnSolutionPath { get; set; } = false;
    
    public Coordinate? Parent { get; set; } = null;

    public List<Coordinate> ToList() => [this];

    public override string ToString() => IsCorrupted ? "X" : IsOnSolutionPath ? "O" : ".";
}

