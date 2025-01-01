using System.Text.RegularExpressions;

namespace Day13;

public partial class ClawMachine
{
    public static Dictionary<Button, int> Costs => new()
    {
        { Button.ButtonA, 3 },
        { Button.ButtonB, 1 },
    };

    public Dictionary<Button, (int dX, int dY)> Buttons { get; init; } = [];
    public (int x, int y) PrizeLocation { get; init; }

    public ClawMachine(string[] input)
    {
        if (input.Length != 3)
            throw new ArgumentException("Expected 3 lines of input", nameof(input));

        var matchA = ButtonARegex().Match(input[0]);
        var matchB = ButtonBRegex().Match(input[1]);
        var matchPrize = PrizeRegex().Match(input[2]);

        if (!matchA.Success)
            throw new ArgumentException("Unable to parse Button A line", nameof(input));
        if (!matchB.Success)
            throw new ArgumentException("Unable to parse Button B line", nameof(input));
        if (!matchPrize.Success)
            throw new ArgumentException("Unable to parse Prize line", nameof(input));

        Buttons.Add(Button.ButtonA, (int.Parse(matchA.Groups[1].Value), int.Parse(matchA.Groups[2].Value)));
        Buttons.Add(Button.ButtonB, (int.Parse(matchB.Groups[1].Value), int.Parse(matchB.Groups[2].Value)));
        PrizeLocation = (int.Parse(matchPrize.Groups[1].Value), int.Parse(matchPrize.Groups[2].Value));
    }

    public Dictionary<Button, long>? Solve(bool useCorrectedPrizeLocation)
    {
        var a1 = Buttons[Button.ButtonA].dX;
        var a2 = Buttons[Button.ButtonA].dY;
        var b1 = Buttons[Button.ButtonB].dX;
        var b2 = Buttons[Button.ButtonB].dY;
        var c1 = PrizeLocation.x + (useCorrectedPrizeLocation ? 10_000_000_000_000 : 0);
        var c2 = PrizeLocation.y + (useCorrectedPrizeLocation ? 10_000_000_000_000 : 0);

        var divisor = (a1 * b2 - b1 * a2);
        var x = (double)(c1 * b2 - b1 * c2) / divisor;
        var y = (double)(a1 * c2 - c1 * a2) / divisor;

        if (IsInteger(x) && IsInteger(y))
        {
            return new Dictionary<Button, long>()
            {
                { Button.ButtonA, (long)x },
                { Button.ButtonB, (long)y }
            };
        }

        return null;
    }

    private static bool IsInteger(double d) => Math.Abs(d % 1) <= (double.Epsilon * 100);

    [GeneratedRegex(@"^Button A: X\+(\d+), Y\+(\d+)$")]
    private static partial Regex ButtonARegex();

    [GeneratedRegex(@"^Button B: X\+(\d+), Y\+(\d+)$")]
    private static partial Regex ButtonBRegex();

    [GeneratedRegex(@"^Prize: X=(\d+), Y=(\d+)$")]
    private static partial Regex PrizeRegex();

    public enum Button
    {
        Start,
        ButtonA,
        ButtonB
    }
}
