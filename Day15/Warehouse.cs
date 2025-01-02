using Serilog;
using System.Text;

namespace Day15;

public class Warehouse
{
    private readonly ILogger logger;
    private readonly Cell[,] cells;
    private readonly Queue<Direction> moves = [];

    public int Height { get; }
    public int Width { get; }

    public (int y, int x) RobotPosition { get; private set; }

    public Warehouse(ILogger logger, string fileName, bool wideMode)
    {
        this.logger = logger;
        
        var lines = File.ReadAllLines(fileName);

        var mapLines = lines.Where(l => l.StartsWith('#')).ToArray();
        
        WideMode = wideMode;
        Height = mapLines.Length;
        Width = mapLines.First().Length * (wideMode ? 2 : 1);

        cells = new Cell[Height, Width];
        
        for (var i = 0; i < Height; i++)
        {
            var line = mapLines[i];
            var cellIndex = 0;

            for (var j=0; j < line.Length; j++)
            {
                cellIndex = j * (wideMode ? 2 : 1);
                var c = line[j];

                cells[i, cellIndex] = new Cell(wideMode)
                {
                    ContainsWall = c == '#',
                    ContainsLeftBoxHalf = c == 'O'
                };
                if (wideMode)
                {
                    cells[i, cellIndex+1] = new Cell(wideMode)
                    {
                        ContainsWall = c == '#',
                        ContainsRightBoxHalf = c == 'O'
                    };
                }

                if (c == '@')
                    RobotPosition = (i, cellIndex);
            }
        }

        var moveLines = lines.Except(mapLines).Where(l => l.Length > 0);
        foreach (var moveLine in moveLines)
        {
            foreach (var move in moveLine)
            {
                moves.Enqueue((Direction)move);
            }
        }
    }

    public void CompleteMoves()
    {
        while (moves.Count > 0)
        {
            var move = moves.Dequeue();
            DoMove(move);
            logger.Debug("New {State}", this);
        }
    }

    private void DoMove(Direction direction)
    {
        logger.Debug("Attempting to move robot {Direction}", direction);

        (int dY, int dX) = direction switch
        {
            Direction.North => (-1, 0),
            Direction.East => (0, 1),
            Direction.South => (1, 0),
            Direction.West => (0, -1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        (int y, int x) newRobotPosition = Advance(RobotPosition);

        var newRobotCell = cells[newRobotPosition.y, newRobotPosition.x];
        if (newRobotCell.ContainsWall)
        {
            logger.Debug("Not moving {Direction} as robot is against a wall", direction);
            return;
        }

        List<(int y, int x)> boxesToPush = [];
        Queue<(int y, int x)> positionsToCheck = new();
        HashSet<(int y, int x)> visitedPositions = [];

        positionsToCheck.Enqueue(newRobotPosition);
        while (positionsToCheck.Count > 0)
        {
            var testPosition = positionsToCheck.Dequeue();
            visitedPositions.Add(testPosition);
            var testCell = GetCell(testPosition);

            if (testCell.ContainsBox)
            {
                if (!boxesToPush.Contains(testPosition))
                {
                    boxesToPush.Add(testPosition);
                    if (WideMode)
                        boxesToPush.Add(GetOtherBoxHalf(testPosition));
                }

                var nextPosition = Advance(testPosition);
                if (!visitedPositions.Contains(nextPosition))
                {
                    positionsToCheck.Enqueue(nextPosition);
                    if (WideMode)
                        positionsToCheck.Enqueue(Advance(GetOtherBoxHalf(testPosition)));
                }
            }
            else if (testCell.ContainsWall)
            {
                logger.Debug("Not moving {Direction} as there's no free space to push {BoxCount} boxes", direction, boxesToPush.Count);
                return;
            }
        }
         
        for (var i = boxesToPush.Count - 1; i >= 0; i--)
        {
            var boxPosition = boxesToPush[i];
            var newBoxPosition = Advance(boxPosition);

            var boxCell = GetCell(boxPosition);
            var newBoxCell = GetCell(newBoxPosition);

            logger.Debug("Moving box from {OldPosition} to {NewPosition}", boxPosition, newBoxPosition);

            if (boxCell.ContainsLeftBoxHalf)
            {
                newBoxCell.ContainsLeftBoxHalf = true;
                boxCell.ContainsLeftBoxHalf = false;
            }
            else if (boxCell.ContainsRightBoxHalf)
            {
                newBoxCell.ContainsRightBoxHalf = true;
                boxCell.ContainsRightBoxHalf = false;
            }
            else
            {
                throw new InvalidOperationException("Pushing a cell that doesn't contain a box?");
            }
        }

        logger.Debug("Moving robot from {OldPosition} to {NewPosition}", RobotPosition, newRobotPosition);
        RobotPosition = newRobotPosition;

        Cell GetCell((int y, int x) position) => cells[position.y, position.x];
        (int y, int x) Advance((int y, int x) position) => (position.y + dY, position.x + dX);
        (int y, int x) GetOtherBoxHalf((int y, int x) position) => 
            GetCell(position).ContainsLeftBoxHalf ? (position.y, position.x + 1) : (position.y, position.x - 1);
    }

    public IEnumerable<(int y, int x)> GetBoxPositions()
    {
        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                if (cells[i, j].ContainsLeftBoxHalf)
                    yield return (i, j);
            }
        }
    }

    public long BoxesGPSSum => GetBoxPositions().Select(PositionToGPS).Sum();

    public bool WideMode { get; }

    private long PositionToGPS((int y, int x) position) => position.y * 100 + position.x;

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("State:");
        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                if ((i, j) == RobotPosition)
                    sb.Append('@');
                else
                    sb.Append(cells[i,j].ToString());
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    internal class Cell(bool WideMode)
    {
        internal bool ContainsWall { get; init; }
        internal bool ContainsLeftBoxHalf { get; set; } = false;
        internal bool ContainsRightBoxHalf { get; set; } = false;
        internal bool IsEmpty => !ContainsWall && !ContainsLeftBoxHalf;
        internal bool ContainsBox => ContainsLeftBoxHalf || ContainsRightBoxHalf;

        public override string ToString()
        {
            if (ContainsWall)
                return "#";
            if (!WideMode && ContainsLeftBoxHalf)
                return "O";
            if (ContainsLeftBoxHalf)
                return "[";
            if (ContainsRightBoxHalf)
                return "]";

            return ".";
        }
    }

    internal enum Direction : byte
    {
        North = (byte)'^',
        East = (byte)'>',
        South = (byte)'v',
        West = (byte)'<'
    }
}
