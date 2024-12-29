namespace Day08;

public class Map
{
    private readonly Location[,] locations;

    public int AntinodeCount => locations.ToEnumerable<Location>().Count(l => l.AntinodeForAntennas.Count > 0);

    public Map(string fileName, bool useResonance)
    {
        var lines = File.ReadAllLines(fileName);

        var width = lines.Max(x => x.Length);
        var height = lines.Length;

        locations = new Location[height, width];

        for (var i = 0; i < height; i++)
        {
            var line = lines[i];
            for (var j = 0; j < line.Length; j++)
            {
                locations[i, j] = line[j] != '.' ? new Location (line[j]) : new Location();
            }
        }

        CalculateAntinodes(useResonance);
    }

    private void CalculateAntinodes(bool isResonant)
    {
        foreach (var antennaCode in locations.ToEnumerable<Location>().Select(l => l.AntennaCode).Where(c => c != null).Cast<char>().Distinct())
        {
            CalculateAntinodesForCode(antennaCode, isResonant);
        }
    }

    private void CalculateAntinodesForCode(char antennaCode, bool useResonance)
    {
        var antennaLocations = GetAntennas(antennaCode).ToList();

        var antennaLocationPairs = antennaLocations.Pairs();

        foreach (var (locationA, locationB) in antennaLocationPairs)
        {
            var dY = locationA.y - locationB.y;
            var dX = locationA.x - locationB.x;

            if (useResonance)
            {
                var newY = locationA.y;
                var newX = locationA.x;

                while (MarkAntinode(antennaCode, newX, newY))
                {
                    newY += dY;
                    newX += dX;
                }
            }
            else
            {
                MarkAntinode(antennaCode, locationA.x + dX, locationA.y + dY);
            }

            if (useResonance)
            {
                var newY = locationB.y;
                var newX = locationB.x;

                while (MarkAntinode(antennaCode, newX, newY))
                {
                    newY -= dY;
                    newX -= dX;
                }
            }
            else
            {
                MarkAntinode(antennaCode, locationB.x - dX, locationB.y - dY);
            }
        }
    }

    private bool MarkAntinode(char antennaCode, int x, int y)
    {
        if (x >= 0 && x < locations.GetLength(0) && y >= 0 && y < locations.GetLength(1))
        {
            locations[y, x].AntinodeForAntennas.Add(antennaCode);
            return true;
        }

        return false;
    }

    private IEnumerable<(int y, int x)> GetAntennas(char antennaCode)
    {
        for (var i = 0; i < locations.GetLength(0); i++)
        {
            for (var j = 0; j < locations.GetLength(1); j++)
            {
                if (locations[i, j].AntennaCode == antennaCode)
                    yield return (i, j);
            }
        }
    }

    public void Print()
    {
        for (var i = 0; i < locations.GetLength(0); i++)
        {
            for (var j = 0; j < locations.GetLength(1); j++)
            {
                locations[i, j].Print();
            }
            Console.WriteLine();
        }
    }
}
