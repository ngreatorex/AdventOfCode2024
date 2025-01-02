using System.Text.RegularExpressions;

namespace Day14;

public partial class Robot
{
    public (int x, int y) Position { get; protected set; }
    public (int dX, int dY) Velocity { get; init; }

    public Robot(string input)
    {
        var match = RobotRegex().Match(input);

        if (!match.Success)
            throw new ArgumentException("Input line was not valid", nameof(input));

        var numbers = match.Groups.Values.Skip(1).Select(g => int.Parse(g.Value)).ToArray();

        Position = (numbers[0], numbers[1]);
        Velocity = (numbers[2], numbers[3]);
    }

    public void Move(int width, int height)
    {
        var newX = (Position.x + Velocity.dX) % width;
        if (newX < 0)
            newX += width;
        
        var newY = (Position.y + Velocity.dY) % height;
        if (newY < 0)
            newY += height;

        Position = (newX, newY);
    }

    [GeneratedRegex(@"^p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)$")]
    private static partial Regex RobotRegex();
}
