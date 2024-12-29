namespace Day6;

public class Position
{
    public Direction Visits { get; set; }
    public bool ContainsObstacle { get; set; }
    public Direction GuardStatus { get; set; }

    public override string ToString()
    {
        if (GuardStatus != Direction.None)
        {
            return GuardStatus switch
            {
                Direction.Up => "^",
                Direction.Right => ">",
                Direction.Down => "v",
                Direction.Left => "<",
                _ => "?"
            };
        }

        if (ContainsObstacle)
            return "#";
        if (Visits != Direction.None)
            return "X";

        return ".";
    }
}
