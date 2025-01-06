using Serilog;
using System.Diagnostics;
using System.Text;

namespace Day20;

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

    protected Maze(ILogger logger)
    {
        this.logger = logger;
    }

    public required Cell[,] cells;
    internal readonly ILogger logger;

    public void Solve()
    {
        var graph = Graph.FromMaze(this);
        var distFromStart = ComputeDistancesFromStart(graph);

        logger.Information("Path with no cheats would take {Cost} picoseconds", distFromStart[graph.End.Cell.Y, graph.End.Cell.X]);

        FindCheats(distFromStart, 2);
        FindCheats(distFromStart, 20);
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

    private void FindCheats(int?[,] distFromStart, int maxCheatDistance)
    {
        var results = new Dictionary<int, int>();
        var stopwatch = Stopwatch.StartNew();

        for (var a = 1; a < cells.GetLength(0); a++)
        {
            for (var b = 1; b < cells.GetLength(1); b++)
            {
                if (cells[a, b].ContainsWall)
                    continue;

                var beforeCheatCostFromStart = distFromStart[a, b] ?? throw new InvalidOperationException("Distance from start is null");

                var minC = MinIndex(a, 0);
                var maxC = MaxIndex(a, 0);
                for (var c = minC; c < maxC; c++)
                {
                    var minD = MinIndex(b, 1);
                    var maxD = MaxIndex(b, 1);
                    for (var d = minD; d < maxD; d++)
                    {
                        if (a == c && b == d)
                            continue;
                        if (cells[c, d].ContainsWall)
                            continue;

                        var cheatCost = ManhattanDistance(a, b, c, d);
                        if (cheatCost < 2 || cheatCost > maxCheatDistance)
                            continue;

                        var afterCheatCostFromStart = distFromStart[c, d] ?? throw new InvalidOperationException("Distance from start is null");

                        var costSaving = afterCheatCostFromStart - (beforeCheatCostFromStart + cheatCost);

                        if (costSaving <= 0)
                            continue;

                        logger.Verbose("Found possible cheat from [{Y1},{X1}] to [{Y2},{X2}] (cheat length {CheatLength}) that saves {CostSaving} picoseconds",
                            a, b, c, d, cheatCost, costSaving);

                        if (!results.TryAdd(costSaving, 1))
                        {
                            results[costSaving]++;
                        }
                    }
                }
            }
        }

        stopwatch.Stop();
        logger.Information("Found {Count} total possible cheats with max length {MaxCheatDistance} in {Duration}", results.Sum(kvp => kvp.Value), maxCheatDistance, stopwatch.Elapsed);
        logger.Debug("Cheats by time saved: {@CheatsByTimeSaved}", results.OrderBy(g => g.Key).ToDictionary(g => g.Key, g => g.Value));
        logger.Information("{Count} cheats with max length {MaxCheatDistance} would save at least 100 picoseconds", results.Where(kvp => kvp.Key >= 100).Sum(kvp => kvp.Value), maxCheatDistance);

        int ManhattanDistance(int y1, int x1, int y2, int x2) => Math.Abs(y1 - y2) + Math.Abs(x1 - x2);
        int MinIndex(int start, int rank) => Math.Clamp(start - maxCheatDistance - 1, 1, cells.GetLength(rank) - 1);
        int MaxIndex(int start, int rank) => Math.Clamp(start + maxCheatDistance + 1, 1, cells.GetLength(rank) - 1);
    }

    private int?[,] ComputeDistancesFromStart(Graph graph)
    {
        var stopwatch = Stopwatch.StartNew();
        var distFromStart = new int?[cells.GetLength(0), cells.GetLength(1)];

        logger.Debug("Pre-computing distances from start for each cell...");

        Parallel.For(1, cells.GetLength(0) - 1, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, i =>
        {
            for (var j = 1; j < cells.GetLength(1) - 1; j++)
            {
                if (cells[i, j].ContainsWall)
                    continue;

                var graphToCell = graph with { End = graph.Nodes[i, j] };
                distFromStart[i, j] = graphToCell.Cost;
            }
        });

        stopwatch.Stop();
        logger.Information("Computed distances from start in {Duration}", stopwatch.Elapsed);

        return distFromStart;
    }

    
}
