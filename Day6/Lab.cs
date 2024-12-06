using System.Text;

namespace Day6;

public class Lab
{
    private Position[,] layout;

    public int VisitedSquareCount => (from Position item in layout where item.Visits != Direction.None select 1).Count();
    
    public IEnumerable<(int row, int column)> GetVisitedSquares()
    {
        for (var row = 0; row < layout.GetLength(0); row++)
        {
            for (var column = 0; column < layout.GetLength(1); column++)
            {
                if (layout[row, column].Visits != Direction.None)
                    yield return (row, column);
            }
        }
    }

    public Lab(string fileName)
    {
        ReadInput(fileName);
    }

    public (int row, int column) GetGuardPosition()
    {
        for (var i = 0; i < layout.GetLength(0); i++)
        {
            for (var j = 0; j < layout.GetLength(1); j++)
            {
                if (layout[i,j].GuardStatus != Direction.None)
                    return (i, j);
            }
        }

        throw new InvalidOperationException("No guard found");
    }

    public CompletionStatus Advance()
    {
        var (row, column) = GetGuardPosition();
        var (nextRow, nextColumn) = GetNextPosition();

        if (nextRow < 0 || nextRow >= layout.GetLength(0))
            return CompletionStatus.GuardExited;
        if (nextColumn < 0 || nextColumn >= layout.GetLength(1))
            return CompletionStatus.GuardExited;

        var currentPositionStatus = layout[row, column];
        var nextPositionStatus = layout[nextRow, nextColumn];

        if (nextPositionStatus.ContainsObstacle)
        {
            currentPositionStatus.GuardStatus = currentPositionStatus.GuardStatus switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => throw new InvalidOperationException("Unknown guard status")
            };
            return CompletionStatus.Incomplete;
        }
        else
        {
            if ((nextPositionStatus.Visits & currentPositionStatus.GuardStatus) != 0)
                return CompletionStatus.Loop;

            nextPositionStatus.GuardStatus = currentPositionStatus.GuardStatus;
            nextPositionStatus.Visits |= currentPositionStatus.GuardStatus;
            currentPositionStatus.Visits |= currentPositionStatus.GuardStatus;
            currentPositionStatus.GuardStatus = Direction.None;

            return CompletionStatus.Incomplete;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int row=0; row < layout.GetLength(0); row++)
        {
            for (int column=0;  column < layout.GetLength(1); column++)
            {
                sb.Append(layout[row, column]);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private (int row, int column) GetNextPosition()
    {
        var (row, column) = GetGuardPosition();
        var currentPositionStatus = layout[row, column];

        return currentPositionStatus.GuardStatus switch
        {
            Direction.Left => (row, column - 1),
            Direction.Up => (row - 1, column),
            Direction.Right => (row, column + 1),
            Direction.Down => (row + 1, column),
            _ => throw new InvalidOperationException("Unexpected guard status"),
        };
    }
    
    private void ReadInput(string fileName)
    {
        var lines = File.ReadLines(fileName).ToList();
        if (lines.Count == 0)
            throw new InvalidDataException("No lines found in input file");

        var width = lines[0].Length;
        var height = lines.Count;

        layout = new Position[height,width];

        for (var i = 0; i < height; i++)
        {
            var line = lines[i];
            for (var j = 0; j < width; j++)
            {
                layout[i, j] = new Position
                {
                    GuardStatus = line[j] switch
                    {
                        '^' => Direction.Up,
                        '>' => Direction.Right,
                        'v' => Direction.Down,
                        '<' => Direction.Left,
                        _ => Direction.None
                    },
                    ContainsObstacle = line[j] == '#',
                    Visits = Direction.None
                };
            }
        }
    }

    public void AddObstacle(int row, int column)
    {
        layout[row, column].ContainsObstacle = true;
    }
}
