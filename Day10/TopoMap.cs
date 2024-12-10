using System.Data.Common;
using System.Text;

namespace Day10;

public record Coordinate(int Row, int Column)
{
    public bool IsValid(int height, int width) => Row >= 0 && Row < height && Column >= 0 && Column < width;
}

public class TopoMap
{
    private Position[,] map;

    public TopoMap(string filename)
    {
        ReadInput(filename);
    }

    private void ReadInput(string fileName)
    {
        var lines = File.ReadLines(fileName).ToList();
        if (lines.Count == 0)
            throw new InvalidDataException("No lines found in input file");

        var width = lines[0].Length;
        var height = lines.Count;

        map = new Position[height,width];

        for (var i = 0; i < height; i++)
        {
            var line = lines[i];
            for (var j = 0; j < width; j++)
            {
                map[i, j] = new Position(int.Parse(line[j].ToString()));
            }
        }   
    }

    public int GetScore()
    {
        return GetTrailheads().Sum(c => GetTrailheadScore(c.Row, c.Column));
    }

    public int GetRatingSum()
    {
        return GetTrailheads().Sum(c => GetTrailheadRating(c.Row, c.Column));
    }

    public IEnumerable<Coordinate> GetTrailheads()
    {
        for (var i=0; i < map.GetLength(0); i++)
        {
            for (var j=0; j < map.GetLength(1); j++)
            {
                if (map[i,j].Height == 0)
                    yield return new Coordinate(i, j);
            }
        }
    }

    private int GetTrailheadRating(int row, int column)
    {
        if (map[row, column].Height != 0)
            throw new ArgumentOutOfRangeException(nameof(row), "Invalid row and/or column for trail head");

        return ContinuePathFrom(0, row, column).Distinct().Count();
    }
    
    private int GetTrailheadScore(int row, int column)
    {
        if (map[row,column].Height != 0)
            throw new ArgumentOutOfRangeException(nameof(row), "Invalid row and/or column for trail head");

        return ContinuePathFrom(0, row, column).Select(p => p.Last()).Distinct().Count();
    }

    private List<List<Coordinate>> ContinuePathFrom(int currentHeight, int row, int column)
    {
        if (currentHeight == 9)
            return [[new Coordinate(row, column)]];

        var up = new Coordinate(row-1, column);
        var right = new Coordinate(row, column+1);
        var down = new Coordinate(row+1, column);
        var left = new Coordinate(row, column-1);

        var candidates = new List<Coordinate>() {up, right, down, left};
        var routesToEnd = candidates.Where(c => c.IsValid(map.GetLength(0), map.GetLength(1)) && GetHeight(c) == currentHeight+1)
            .SelectMany(c => ContinuePathFrom(currentHeight+1, c.Row, c.Column))
            .ToList();

        return routesToEnd.Select(c => c.Prepend(new Coordinate(row, column)).ToList()).ToList();
    }

    private int GetHeight(Coordinate c) => map[c.Row,c.Column].Height;

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int row=0; row < map.GetLength(0); row++)
        {
            for (int column=0; column < map.GetLength(1); column++)
            {
                sb.Append(map[row, column]);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}