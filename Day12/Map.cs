using Serilog;
using System.Data;
using System.Text;

namespace Day12;

public class Map
{
    private readonly char[,] map;
    private readonly ILogger log;

    public Map(ILogger log, string fileName)
    {
        this.log = log;
        var lines = File.ReadAllLines(fileName);

        if (lines.Length == 0)
            throw new InvalidDataException("No input found");

        var height = lines.Length;
        var width = lines[0].Length;

        if (!lines.All(l => l.Length == width))
            throw new InvalidDataException("Not all lines are the same length");

        if (height != width)
            throw new InvalidDataException("Map is not square");

        map = new char[height, width];

        for (var row = 0; row < map.GetLength(0); row++)
        {
            for (var col = 0; col < map.GetLength(1); col++)
            {
                map[row, col] = lines[row][col];
            }
        }

        _regions = new(() => GetAllRegions().ToList());

        _costs = new(() =>
        {
            log.Information("Calculating costs...");
            return _regions.Value
                        .Select(t => (t.plot, t.region))
                        .GroupBy(t => t.plot)
                        .ToDictionary(g => g.Key, g => (IList<IDictionary<string, int>>)g.Select(e => new Dictionary<string, int>() {
                            { "Area", CalculateArea(e.region) },
                            { "Perimeter", CalculatePerimeter(e.region) }
                        }).Cast<IDictionary<string, int>>().ToList());
        });

        _discountedCosts = new(() =>
        {
            log.Information("Calculating discounted costs...");
            return _regions.Value
                        .Select(t => (t.plot, t.region))
                        .GroupBy(t => t.plot)
                        .ToDictionary(g => g.Key, g => (IList<IDictionary<string, int>>)g.Select(e => new Dictionary<string, int>() {
                            { "Area", CalculateArea(e.region) },
                            { "Sides", CountCorners(log, e.region) }
                        }).Cast<IDictionary<string, int>>().ToList());
        });
    }

    public int Height => map.GetLength(0);
    public int Width => map.GetLength(1);

    public int TotalCost => Costs.Values.Select(cost => cost.Sum(e => e.Values.Aggregate((a, b) => a * b))).Sum();
    public int TotalDiscountedCost => DiscountedCosts.Values.Select(cost => cost.Sum(e => e.Values.Aggregate((a, b) => a * b))).Sum();

    private readonly Lazy<IDictionary<char, IList<IDictionary<string, int>>>> _costs;
    public IDictionary<char, IList<IDictionary<string, int>>> Costs => _costs.Value;

    private readonly Lazy<IDictionary<char, IList<IDictionary<string, int>>>> _discountedCosts;
    public IDictionary<char, IList<IDictionary<string, int>>> DiscountedCosts => _discountedCosts.Value;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        for (var row = 0; row < map.GetLength(0); row++)
        {
            for (var col = 0; col < map.GetLength(1); col++)
            {
                sb.Append(map[row, col]);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private readonly Lazy<List<(char plot, (int row, int col) coords, bool[,] region)>> _regions;
    private IEnumerable<(char plot, (int row, int col) coords, bool[,] region)> GetAllRegions()
    {
        var visitedCells = new bool[map.GetLength(0), map.GetLength(1)];
        log.Information("Calculationg regions...");

        for (var i = 0; i < map.GetLength(0); i++)
        {
            for (var j = 0; j < map.GetLength(1); j++)
            {
                if (visitedCells[i, j])
                    continue;

                var newRegion = FindRegionContaining(i, j);

                yield return (map[i, j], (i, j), newRegion);
                MergeRegions(visitedCells, newRegion);
            }
        }
    }

    private bool[,] FindRegionContaining(int row, int col)
    {
        var dimensions = (rows: map.GetLength(0), cols: map.GetLength(1));
        var visited = new bool[dimensions.rows, dimensions.cols];
        var stack = new Stack<(int row, int col)>();
        var plot = map[row, col];

        stack.Push((row, col));

        while (stack.Count > 0)
        {
            var coords = stack.Pop();

            if (!visited[coords.row, coords.col])
            {
                visited[coords.row, coords.col] = true;

                foreach (var (dy, dx) in neighbours)
                {
                    var neighbourCoords = (row: coords.row + dy, col: coords.col + dx);
                    if (IsValidCoords(neighbourCoords, dimensions) && map[neighbourCoords.row, neighbourCoords.col] == plot)
                        stack.Push(neighbourCoords);
                }
            }
        }

        return visited;
    }

    private static readonly List<(int dy, int dx)> neighbours = [
        (-1, 0), (0, -1), (1, 0), (0, 1)
    ];

    private static readonly Dictionary<string, List<(int dy, int dx)>> cornerLookaheads = new() {
        { "NorthWest", [(0, -1), (-1, 0)] },
        { "NorthEast", [(-1, 0), (0, 1)] },
        { "SouthEast", [(0, 1), (1, 0)] },
        { "SouthWest", [(1, 0), (0, -1)] }
    };

    private static bool IsValidCoords((int row, int col) coords, (int rows, int cols) dimensions)
    {
        return coords.row >= 0 && coords.row < dimensions.rows
            && coords.col >= 0 && coords.col < dimensions.cols;
    }

    private static void PrintRegion(ILogger log, bool[,] region)
    {
        var sb = new StringBuilder();

        for (var row = 0; row < region.GetLength(0); row++)
        {
            for (var col = 0; col < region.GetLength(1); col++)
            {
                sb.Append(region[row, col] ? '1' : '0');
            }
            sb.AppendLine();
        }

        log.Verbose("Region: {@Region}", sb.ToString());
    }

    private static int CountCorners(ILogger log, bool[,] region)
    {
        var cornerCount = 0;
        var dimensions = (rows: region.GetLength(0), cols: region.GetLength(1));
        PrintRegion(log, region);

        for (var i = 0; i < dimensions.rows + 1; i++)
        {
            for (var j = 0; j < dimensions.cols + 1; j++)
            {
                cornerCount += CountCornersAt(i, j);
            }
        }

        return cornerCount;

        bool GetCell(int row, int col)
        {
            return IsValidCoords((row, col), dimensions) && region[row, col];
        }

        int CountCornersAt(int row, int col)
        {
            var result = 0;

            foreach (var lookahead in cornerLookaheads)
            {
                var current = GetCell(row, col);
                if (!current)
                    continue;

                var north = GetCell(row + lookahead.Value[0].dy, col + lookahead.Value[0].dx);
                var west = GetCell(row + lookahead.Value[1].dy, col + lookahead.Value[1].dx);
                var northWest = GetCell(row + lookahead.Value[0].dy + lookahead.Value[1].dy, col + lookahead.Value[0].dx + lookahead.Value[1].dx);

                if (north != current && west != current)
                {
                    result++;
                    log.Debug("Found {Direction} facing {CornerType} corner at ({Row},{Col})", lookahead.Key, "outer", row, col);
                }
                if (north == current && west == current && northWest != current)
                {
                    result++;
                    log.Debug("Found {Direction} facing {CornerType} corner at ({Row},{Col})", lookahead.Key, "inner", row, col);
                }
            }

            if (result > 0)
            {
                log.Debug("Found {CornerCount} corners at ({Row},{Col})", result, row, col);
            }

            return result;
        }
    }

    private static void MergeRegions(bool[,] target, bool[,] source)
    {
        for (var i = 0; i < target.GetLength(0); i++)
        {
            for (var j = 0; j < target.GetLength(1); j++)
            {
                target[i, j] |= source[i, j];
            }
        }
    }

    private static int CalculatePerimeter(bool[,] region)
    {
        var inRegion = new bool[] { false, false };
        var currentTransitionCount = 0;

        for (var i = 0; i < region.GetLength(0); i++)
        {
            inRegion = [false, false];
            for (var j = 0; j < region.GetLength(1); j++)
            {
                for (var k = 0; k < inRegion.Length; k++)
                {
                    var nextCell = k == 0 ? region[i, j] : region[j, i];
                    if (inRegion[k] && !nextCell || !inRegion[k] && nextCell)
                    {
                        inRegion[k] = !inRegion[k];
                        currentTransitionCount++;
                    }
                }

            }
            currentTransitionCount += inRegion.Count(b => b);
        }

        return currentTransitionCount;
    }

    private static int CalculateArea(bool[,] region)
    {
        return region.Cast<bool>().Count(c => c);
    }
}
