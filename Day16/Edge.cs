namespace Day16;

public record Edge(Node Child, Move[] Moves)
{
    public int Weight => Moves.Sum(m => MazeInfo.costs[m]);

    public override string ToString() => string.Join(" ", Moves.Select(m => m.ToString()));
}