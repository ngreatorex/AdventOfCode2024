using Serilog;
using System.Text;

namespace Day14;

public class Floor
{
    private readonly List<Robot> robots = [];
    private readonly ILogger logger;

    public Floor(ILogger logger, string fileName, int width, int height)
    {
        var lines = File.ReadAllLines(fileName);
        
        foreach (var line in lines)
        {
            robots.Add(new Robot(line));
        }

        this.logger = logger;
        Width = width;
        Height = height;

        map = new ResettableLazy<int[,]>(() =>
        {
            var map = new int[Height, Width];
            foreach (var robot in robots)
            {
                map[robot.Position.y, robot.Position.x]++;
            }

            return map;
        });
    }

    public List<Robot> Robots => robots;
    public int Width { get; }
    public int Height { get; }

    public int MoveUntilPossibleXmasTree()
    {
        var moves = 0;
        while (!ToString().Contains("1111111111111111111111111111111"))
        {
            moves++;
            Move();
        }

        return moves;
    }

    public void Move()
    {
        map.Reset();
        Parallel.ForEach(robots, robot => robot.Move(Width, Height));
    }

    public void Move(int times)
    {
        for (int i = 0; i < times; i++)
        {
            Move();
        }
    }

    public int SafetyFactor
    {
        get
        {
            var quadrantFactors = new int[4];

            for (var i=0; i < Height; i++)
            {
                for (var j=0; j < Width; j++)
                {
                    var quadrant = GetQuadrant(i, j);
                    if (quadrant == null)
                        continue;

                    if (Map[i,j] > 0)
                        quadrantFactors[quadrant.Value] += Map[i,j];
                }
            }

            logger.Information("Quadrant counts: {@QuadrantCounts}", quadrantFactors);

            return quadrantFactors[0] * quadrantFactors[1] * quadrantFactors[2] * quadrantFactors[3];
        }
    }

    private int? GetQuadrant(int y, int x)
    {
        int zeroBasedMiddleY = Height / 2 + (Height % 2 == 1 ?  0 : -1);
        int zeroBasedMiddleX = Width / 2 + (Width % 2 == 1 ?  0 : -1);

        if (y < zeroBasedMiddleY && x < zeroBasedMiddleX)
            return 0;
        if (y < zeroBasedMiddleY && x > zeroBasedMiddleX)
            return 1;
        if (y > zeroBasedMiddleY && x < zeroBasedMiddleX)
            return 2;
        if (y > zeroBasedMiddleY && x > zeroBasedMiddleX)
            return 3;

        return null;
    }

    private ResettableLazy<int[,]> map;

    private int[,] Map => map.Value;

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("current state:");

        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                sb.Append(Map[i, j] > 0 ? Map[i, j].ToString() : ".");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
