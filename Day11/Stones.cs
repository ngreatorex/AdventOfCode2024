namespace Day11;

public class Stones
{
    private readonly Func<IStoneStorage> storageFactory;
    public Stones(Func<IStoneStorage> storageFactory, string fileName)
    {
        this.storageFactory = storageFactory;
        var lines = File.ReadAllLines(fileName);

        if (lines.Length != 1)
            throw new InvalidOperationException("Expected single line of input");

        var line = lines[0];
        var values = line.Split(' ').Select(long.Parse).ToArray();

        Values = storageFactory();
        foreach (var v in values)
            Values.Add(v);
    }

    public IStoneStorage Values { get; private set; }

    public long MaxStoneValue { get; private set; }

    public void Blink()
    {
        MaxStoneValue = 0;
        var output = storageFactory();
     
        foreach ((var stone, var count) in Values)
        {
            if (stone > MaxStoneValue)
                MaxStoneValue = stone;

            if (stone == 0)
            {
                output.Add(1, count);
                continue;
            }

            var digitCount = Math.Floor(Math.Log10(stone)) + 1;
            if (digitCount % 2 == 0)
            {
                long divisor = (long)Math.Pow(10, digitCount / 2);

                (var stoneOne, var stoneTwo) = Math.DivRem(stone, divisor);

                output.Add(stoneOne, count);
                output.Add(stoneTwo, count);
                continue;
            }

            output.Add(checked(stone * 2024), count);
        }

        Values = output;
    }

    public override string ToString() => Values?.ToString() ?? "";
}
