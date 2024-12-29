using System.Text.RegularExpressions;

var lines = File.ReadLinesAsync("Input.txt");
var rules = new List<Rule>();
var updates = new List<int[]>();

var ruleRegex = new Regex(@"^(\d*)\|(\d*)$", RegexOptions.Compiled);
var updateRegex = new Regex(@"^(\d*,)+\d+$", RegexOptions.Compiled);

await foreach (var line in lines)
{
    var ruleMatch = ruleRegex.Match(line);
    var updateMatch = updateRegex.Match(line);

    if (ruleMatch.Success)
    {
        rules.Add(new Rule { PageA = int.Parse(ruleMatch.Groups[1].Value), PageB = int.Parse(ruleMatch.Groups[2].Value) });
    }
    else if (updateMatch.Success)
    {
        updates.Add(updateMatch.Value.Split(',').Select(int.Parse).ToArray());
    }
}

var resortedUpdates = new List<int[]>();
var correctUpdates = new List<int[]>();
foreach (var update in updates)
{
    if (IsUpdateCorrectlyOrdered(update))
    {
        correctUpdates.Add(update);
    }
    else
    {
        var newUpdate = new int[update.Length];
        Array.Copy(update, newUpdate, update.Length);

        Array.Sort(newUpdate, (a, b) =>
        {
            if (rules.Any(r => r.PageA == a && r.PageB == b))
                return 1;
            if (rules.Any(r => r.PageA == b && r.PageB == a))
                return -1;
            return 0;
        });

        resortedUpdates.Add(newUpdate);
    }
}

var middlePageTotal = correctUpdates.Select(u => u[u.Length / 2]).Sum();
Console.WriteLine($"Sum of middle page numbers of originally correct updates is {middlePageTotal}");

var resortedMiddlePageTotal = resortedUpdates.Select(u => u[u.Length / 2]).Sum();
Console.WriteLine($"Sum of middle page numbers of re-sorted updates is {resortedMiddlePageTotal}");


bool IsUpdateCorrectlyOrdered(int[] update)
{
    for (var i = 0; i < update.Length; i++)
    {
        for (var j = 0; j < i; j++)
        {
            if (rules.Any(r => r.PageA == update[i] && r.PageB == update[j]))
                return false;
        }
        for (var j = i+1; j < update.Length; j++)
        {
            if (rules.Any(r => r.PageA == update[j] && r.PageB == update[i]))
                return false;
        }
    }

    return true;
}

