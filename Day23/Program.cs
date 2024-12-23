
using System.Text.RegularExpressions;
using Day23;

var computers = new Dictionary<string, Computer>();
var links = new List<Link>();

var lines = File.ReadLinesAsync("Input.txt");
var regex = new Regex(@"^([a-z]{2})-([a-z]{2})$");

await foreach (var line in lines)
{
    var match = regex.Match(line);
    if (match.Success)
    {
        var a = match.Groups[1].Value;
        var b = match.Groups[2].Value;

        if (!computers.ContainsKey(a))
            computers[a] = new Computer(a);
        if (!computers.ContainsKey(b))
            computers[b] = new Computer(b);

        links.Add(new Link(computers[a], computers[b]));
    }
}

var startingComputers = computers.Values.Select(CreateList).ToList();
var distanceOneComputers = GetPathsFrom(startingComputers, true).Select(ConvertToList).ToList();
var distanceTwoComputers = GetPathsFrom(distanceOneComputers, true).Select(ConvertToList).ToList();
var distanceThreeComputers = GetPathsFrom(distanceTwoComputers, true).Select(ConvertToList).ToList();

var sets = distanceThreeComputers.Where(p => p.Last() == p.First()).Select(p => p.Distinct())
    .Where(p => p.Any(c => c.Identifier.StartsWith('t'))).ToList();
var orderedSets = sets.Select(p => p.Order().ToList()).ToList();
var distinctSets = orderedSets.Distinct(new PathComparer()).ToList();

foreach (var set in distinctSets)
{
    Console.WriteLine($"Found possibility: {string.Join(',', set)}");
}

Console.WriteLine($"Found {distinctSets.Count} possibilities");

var largestSet = computers.Values.Select(RecursePathsFrom).Select(ConvertToList).MaxBy(p => p.Count);
Console.WriteLine($"Largest set is of {largestSet.Count} computers: {string.Join(',', largestSet)}. Password is {string.Join(',', largestSet.OrderBy(c => c.Identifier))}");


List<Computer> ConvertToList(IEnumerable<Computer> e) => [.. e];
List<Computer> CreateList(Computer c) => [c];
IEnumerable<IEnumerable<Computer>> GetPathsFrom(List<List<Computer>> input, bool allowDuplicates)
{
    foreach (var path in input)
    {
        var lastComputer = path.Last();
        foreach (var link in links.Where(l => l.A == lastComputer || l.B == lastComputer))
        {
            var nextComputer = link.GetOtherEndOfLinkFrom(lastComputer);
            if (nextComputer == null)
                continue;
            if (!allowDuplicates && path.Contains(nextComputer))
                yield return path;
            yield return path.Append(nextComputer);
        }
    }
}

IEnumerable<Computer> RecursePathsFrom(Computer c)
{
    var paths = new List<HashSet<Computer>>();
    long lastSum = 0;
    paths.Add([c]);
    long newSum = 1;

    while (newSum != lastSum)
    {
        lastSum = newSum;
        paths = GetPathsFrom(paths.Select(s => s.ToList()).ToList(), false).Select(p => new HashSet<Computer>(p))
            .Where(p => p.SelectMany(x => p, (x, y) => Tuple.Create(x, y))
                .Where(tuple => tuple.Item1 != tuple.Item2).All(tuple =>
            {
                return LinkExists(tuple.Item1, tuple.Item2);
            }))
            .Distinct(new NodeSetComparer()).ToList();
        var maxCount = paths.Max(p => p.Count);
        paths = paths.Where(p => p.Count == maxCount).ToList();
        newSum = paths.Sum(p => p.Count);
    }

    return paths[0];
}

bool LinkExists(Computer a, Computer b) => links.Any(l => l.A == a && l.B == b || l.A == b && l.B == a);

HashSet<Computer> BuildNodeSet(Computer c)
{
    var visitedComputers = new HashSet<Computer>();
    var set = new HashSet<Computer>();

    var q = new Queue<Computer>();
    q.Enqueue(c);
    while (q.Count > 0)
    {
        var current = q.Dequeue();

        visitedComputers.Add(current);

        if (set.All(d => links.Any(l => l.A == current && l.B == d || l.A == d && l.B == current)))
            set.Add(current);

        foreach (var link in links)
        {
            var connectedComputer = link.GetOtherEndOfLinkFrom(current);
            if (connectedComputer != null && !visitedComputers.Contains(connectedComputer))
                q.Enqueue(connectedComputer);
        }
    }

    return set;
}