namespace Day08;

public class Location
{
    public char? AntennaCode { get; init; }

    public List<char> AntinodeForAntennas { get; init; } = [];

    public Location()
    {
        AntennaCode = null;
    }

    public Location(char antennaCode)
    {
        AntennaCode = antennaCode;
    }

    public override string ToString()
    {
        if (AntennaCode != null)
            return $"{AntennaCode}";

        if (AntinodeForAntennas.Count > 0)
            return "*";

        return ".";
    }

    public void Print()
    {
        var originalColor = Console.ForegroundColor;

        if (AntinodeForAntennas.Count > 0)
            Console.ForegroundColor = ConsoleColor.Green;
        else
            Console.ForegroundColor = ConsoleColor.Red;

        Console.Write(AntennaCode ?? (AntinodeForAntennas.Count > 0 ? AntinodeForAntennas[0] : '.'));

        Console.ForegroundColor = originalColor;
    }
}
