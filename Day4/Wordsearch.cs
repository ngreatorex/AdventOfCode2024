using System.Text;

namespace Day04;

public class Wordsearch
{
    private readonly char[,] grid;

    public Wordsearch(string filename)
    {
        var lines = File.ReadAllLines(filename);

        if (lines.Length == 0)
            throw new ArgumentException("Input has no lines", nameof(filename));

        var width = lines[0].Length;
        grid = new char[lines.Length, width];

        for (var i = 0; i < lines.Length; i++)
        {
            for (var j = 0; j < width; j++)
            {
                grid[i, j] = lines[i][j];
            }
        }
    }

    public int CountAllMatches(MatchType type)
    {
        int total = 0;

        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                total += CheckForMatchAtPosition(type, i, j).Count();
            }

        return total;
    }

    private IEnumerable<Direction> CheckForMatchAtPosition(MatchType type, int startRow, int startColumn)
    {
        var gridHeight = grid.GetLength(0);
        var gridWidth = grid.GetLength(1);

        var posToCheck = GetPossibilities(type); 

        return posToCheck.Where(p => p.Value.All(t => IsValidPos(t.row, t.col) && Matches(t.row, t.col, t.value))).Select(p => p.Key);

        bool IsValidPos(int row, int col) => startRow + row < gridHeight && startColumn + col < gridWidth;
        bool Matches(int row, int col, char value) => grid[startRow + row, startColumn + col] == value;
    }

    private static Dictionary<Direction, List<(int row, int col, char value)>> GetPossibilities(MatchType type)
    {
        return type switch
        {
            MatchType.Xmas => new Dictionary<Direction, List<(int row, int col, char value)>>()
            {
                [Direction.East] = [
                    (0, 0, 'X'),
                    (0, 1, 'M'),
                    (0, 2, 'A'),
                    (0, 3, 'S')
                ],
                [Direction.West] = [
                    (0, 3, 'X'),
                    (0, 2, 'M'),
                    (0, 1, 'A'),
                    (0, 0, 'S')
                ],
                [Direction.South] = [
                    (0, 0, 'X'),
                    (1, 0, 'M'),
                    (2, 0, 'A'),
                    (3, 0, 'S')
                ],
                [Direction.North] = [
                    (3, 0, 'X'),
                    (2, 0, 'M'),
                    (1, 0, 'A'),
                    (0, 0, 'S')
                ],
                [Direction.SouthEast] = [
                    (0, 0, 'X'),
                    (1, 1, 'M'),
                    (2, 2, 'A'),
                    (3, 3, 'S')
                ],
                [Direction.NorthWest] = [
                    (3, 3, 'X'),
                    (2, 2, 'M'),
                    (1, 1, 'A'),
                    (0, 0, 'S')
                ],
                [Direction.SouthWest] = [
                    (0, 3, 'X'),
                    (1, 2, 'M'),
                    (2, 1, 'A'),
                    (3, 0, 'S')
                ],
                [Direction.NorthEast] = [
                    (3, 0, 'X'),
                    (2, 1, 'M'),
                    (1, 2, 'A'),
                    (0, 3, 'S')
                ]
            },
            MatchType.MasInAnX => new Dictionary<Direction, List<(int row, int col, char value)>>()
            {
                [Direction.SouthEast] = [
                    (0, 0, 'M'),
                    (1, 1, 'A'),
                    (2, 2, 'S'),
                    (2, 0, 'M'),
                    (0, 2, 'S')
                ],
                [Direction.NorthEast] = [
                    (0, 0, 'M'),
                    (1, 1, 'A'),
                    (2, 2, 'S'),
                    (2, 0, 'S'),
                    (0, 2, 'M')
                ],
                [Direction.NorthWest] = [
                    (0, 0, 'S'),
                    (1, 1, 'A'),
                    (2, 2, 'M'),
                    (2, 0, 'M'),
                    (0, 2, 'S')
                ],
                [Direction.SouthWest] = [
                    (0, 0, 'S'),
                    (1, 1, 'A'),
                    (2, 2, 'M'),
                    (2, 0, 'S'),
                    (0, 2, 'M')
                ],
            },
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (var i = 0; i < grid.GetLength(0); i++)
        {
            for (var j = 0; j < grid.GetLength(1); j++)
            {
                sb.Append(grid[i, j]);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
