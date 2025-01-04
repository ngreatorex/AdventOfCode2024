using Serilog;
using System.Diagnostics;
using System.Text;

namespace Day16;

public partial class Maze
{
    public static async Task<Maze> OpenMaze(ILogger logger, string fileName)
    {
        var maze = new Maze(logger)
        {
            cells = await Load(logger, fileName)
        };

        logger.Debug("Initial {State}", maze);

        return maze;
    }

    protected static async Task<Cell[,]> Load(ILogger logger, string fileName)
    {
        var lines = await File.ReadAllLinesAsync(fileName);

        Debug.Assert(lines.Length > 0);

        var cells = new Cell[lines.Length, lines[0].Length];

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            for (var j = 0; j < line.Length; j++)
            {
                var c = line[j];
                cells[i, j] = new Cell(i, j)
                {
                    IsStart = c == 'S',
                    IsEnd = c == 'E',
                    ContainsWall = c == '#'
                };
            }
        }

        logger.Debug("Loaded maze from {FileName}", fileName);
        return cells;
    }

    private static Move DirectionToMove(Direction direction)
    {
        return direction switch
        {
            Direction.North => Move.MoveNorth,
            Direction.East => Move.MoveEast,
            Direction.South => Move.MoveSouth,
            Direction.West => Move.MoveWest,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };
    }

    protected Maze(ILogger logger)
    {
        this.logger = logger;
    }

    public required Cell[,] cells;
    private readonly ILogger logger;

    public void Solve()
    {
        var graph = TransformMazeToGraph();
        var paths = graph.GetShortestPaths().ToList();

        foreach (var (_, Moves, Cost) in paths)
        {
            var path = Moves.ToList();

            logger.Information("Solution: {Path}", path);
            logger.Information("{MazeWithPath}", MazeWithPath(path));
            logger.Information("Cost: {Cost}", Cost);
        }

        var allVisitedCells = paths.SelectMany(t => t.Cells).Distinct().ToList();
        logger.Information("Visited cells: {AllVisitedCells}", allVisitedCells);
        logger.Information("{MazeWithVisitedCells}", MazeWithVisitedCells(allVisitedCells));
        logger.Information("Found {PathCount} shortest paths that visit {CellCount} cells", paths.Count, allVisitedCells.Count);
    }

    public string MazeWithVisitedCells(List<Cell> visitedCells)
    {
        var chars = new char[cells.GetLength(0), cells.GetLength(1)];

        for (var i = 0; i < cells.GetLength(0); i++)
        {
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                chars[i, j] = visitedCells.Contains(cells[i, j]) ? 'O' : cells[i, j].ToChar();
            }
        }

        var sb = new StringBuilder();
        sb.AppendLine("Maze with visited cells:");

        for (var i = 0; i < chars.GetLength(0); i++)
        {
            for (var j = 0; j < chars.GetLength(1); j++)
            {
                sb.Append(chars[i, j]);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public string MazeWithPath(List<Move> moves)
    {
        var chars = new char[cells.GetLength(0), cells.GetLength(1)];

        for (var i = 0; i < cells.GetLength(0); i++)
        {
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                chars[i, j] = cells[i, j].ToChar();
            }
        }

        var currentCell = cells.Cast<Cell>().Single(c => c.IsStart);
        var pathIndex = 0;
        while (!currentCell.IsEnd && pathIndex < moves.Count)
        {
            var nextMove = moves[pathIndex++];
            if (nextMove == Move.RotateCounterClockwise || nextMove == Move.RotateClockwise)
                continue;

            chars[currentCell.Y, currentCell.X] = MoveToChar(nextMove);

            var (_, y, x) = MazeInfo.possibleMoves.First(m => m.direction == MoveToDirection(nextMove));
            currentCell = cells[currentCell.Y + y, currentCell.X + x];
        }

        var sb = new StringBuilder();
        sb.AppendLine("Maze with path:");

        for (var i = 0; i < chars.GetLength(0); i++)
        {
            for (var j = 0; j < chars.GetLength(1); j++)
            {
                sb.Append(chars[i, j]);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static Direction MoveToDirection(Move nextMove)
    {
        return nextMove switch
        {
            Move.MoveNorth => Direction.North,
            Move.MoveEast => Direction.East,
            Move.MoveSouth => Direction.South,
            Move.MoveWest => Direction.West,
            _ => throw new ArgumentOutOfRangeException(nameof(nextMove)),
        };
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("State:");

        for (var i = 0; i < cells.GetLength(0); i++)
        {
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                sb.Append(cells[i, j].ToChar());
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private Graph TransformMazeToGraph()
    {
        var directedNodes = new Dictionary<Cell, Dictionary<Direction, Node>>();
        Node? startNode = null, endNode = null;
        
        for (var i = 0; i < cells.GetLength(0); i++)
        {
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                var cell = cells[i, j];
                if (cell.ContainsWall)
                    continue;
                
                directedNodes[cell] = Enum.GetValues<Direction>().Select(d => new Node(d, cell, [])).ToDictionary(n => n.Facing, n => n);

                foreach (var n1 in directedNodes[cell])
                {
                    n1.Value.Edges.AddRange(directedNodes[cell].Except([n1]).Select(n2 => new Edge(n2.Value, MazeInfo.directionPairs[n1.Key][n2.Key])));
                }

                if (cell.IsStart)
                    startNode = directedNodes[cell][Direction.East];
                if (cell.IsEnd)
                    endNode = directedNodes[cell][Direction.East];
            }
        }

        for (var i = 0; i < cells.GetLength(0); i++)
        {
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                var cell = cells[i, j];
                
                if (cell.ContainsWall)
                    continue;

                var neighbours = GetNeighbours((i, j));
                foreach (var (direction, y, x) in neighbours)
                {
                    var neighbourCell = cells[y, x];

                    directedNodes[cell][direction].Edges.Add(new Edge(directedNodes[neighbourCell][direction], [DirectionToMove(direction)]));
                }
            }
        }

        if (startNode == null || endNode == null)
            throw new InvalidOperationException("Either start or end node is null");

        return new Graph(logger, directedNodes.Values.SelectMany(kvp => kvp.Values).ToList(), startNode, endNode);
    }

    private (Direction direction, int y, int x)[] GetNeighbours((int y, int x) position)
    {
        var pms = MazeInfo.possibleMoves.Select(pm => (pm.direction, position.y + pm.y, position.x + pm.x)).ToArray();
        var valid = pms.Where(IsValidMove).ToArray();
        return valid;

        bool IsValidMove((Direction direction, int y, int x) move) => IsValidPosition(move.y, move.x) && !cells[move.y, move.x].ContainsWall;
        bool IsValidPosition(int y, int x) => y >= 0 && y < cells.GetLength(0) && x >= 0 && x < cells.GetLength(1);
    }

    private static char MoveToChar(Move m)
    {
        return m switch
        {
            Move.MoveNorth => '^',
            Move.MoveEast => '>',
            Move.MoveSouth => 'v',
            Move.MoveWest => '<',
            Move.RotateClockwise => '/',
            Move.RotateCounterClockwise => '\\',
            _ => throw new NotImplementedException(),
        };
    }
}
