using System.Text.RegularExpressions;

var lines = await File.ReadAllTextAsync("Input.txt");
var regex = new Regex(@"(mul\(([0-9]{1,3}),([0-9]{1,3})\)|do\(\)|don't\(\))", RegexOptions.Compiled);

var matches = regex.Matches(lines);
Console.WriteLine($"Found {matches.Count} valid instructions");

var total = matches.Where(match => match.Groups[0].Value.StartsWith("mul")).Select(match => int.Parse(match.Groups[2].Value) * int.Parse(match.Groups[3].Value)).Sum();
Console.WriteLine($"Result is {total}");

var currentlyEnabled = true;
var runningTotal = 0;
foreach (var match in matches.Cast<Match>())
{
    if (match.Groups[0].Value.StartsWith("mul("))
    {
        if (currentlyEnabled)
            runningTotal += int.Parse(match.Groups[2].Value) * int.Parse(match.Groups[3].Value);
    }
    else if (match.Groups[0].Value.Equals("don't()"))
    {
        currentlyEnabled = false;
    }
    else if (match.Groups[0].Value.Equals("do()"))
    {
        currentlyEnabled = true;
    }
    else
    {
        throw new InvalidOperationException("Unknown match");
    }
}

Console.WriteLine($"Result with conditional evaluation is {runningTotal}");