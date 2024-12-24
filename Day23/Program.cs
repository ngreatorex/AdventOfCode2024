
using System.Collections.Concurrent;
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

var startingComputers = computers.Values.Select(c => c.ToList()).ToList();
var distanceOneComputers = GetPathsFrom(startingComputers, true);
var distanceTwoComputers = GetPathsFrom(distanceOneComputers, true);
var distanceThreeComputers = GetPathsFrom(distanceTwoComputers, true);

var sets = distanceThreeComputers.Where(p => p.Last() == p.First()).Select(p => p.Distinct())
    .Where(p => p.Any(c => c.Identifier.StartsWith('t'))).ToList();
var orderedSets = sets.Select(p => p.Order().ToList()).ToList();
var distinctSets = orderedSets.Distinct(new PathComparer()).ToList();

foreach (var set in distinctSets)
{
    Console.WriteLine($"Found possibility: {string.Join(',', set)}");
}

Console.WriteLine($"Found {distinctSets.Count} possibilities");

var largestSet = computers.Values.AsParallel().Select(BuildNodeSet).MaxBy(p => p.Count);
if (largestSet is null)
    throw new InvalidOperationException("Largest set is null");
Console.WriteLine($"Largest set is of {largestSet.Count} computers. Password is {string.Join(',', largestSet.OrderBy(c => c.Identifier))}");

return;

List<List<Computer>> GetPathsFrom(List<List<Computer>> input, bool allowDuplicates)
{
    ConcurrentBag<List<Computer>> result = [];
    
    Parallel.ForEach(input, path =>
    {
        var lastComputer = path.Last();
        Parallel.ForEach(links.Where(l => l.A == lastComputer || l.B == lastComputer), link =>
        {
            var nextComputer = link.GetOtherEndOfLinkFrom(lastComputer);
            if (nextComputer == null)
                return;
            if (!allowDuplicates && path.Contains(nextComputer))
                result.Add(path);
            result.Add(path.Append(nextComputer).ToList());
        });
    });

    return result.ToList();
}

HashSet<Computer> BuildNodeSet(Computer computer)
{
    var visitedComputers = new HashSet<Computer>();
    
    var q = new Queue<Computer>();
    q.Enqueue(computer);
    while (q.Count > 0)
    {
        var current = q.Dequeue();

        visitedComputers.Add(current);

        foreach (var link in links)
        {
            var connectedComputer = link.GetOtherEndOfLinkFrom(current);
            if (connectedComputer == null || visitedComputers.Contains(connectedComputer) || q.Contains(connectedComputer))
                continue;

            var newSet = new HashSet<Computer>(visitedComputers) { connectedComputer };
            if (IsSetConnected(newSet))
                q.Enqueue(connectedComputer);
        }
    }

    var set = new HashSet<Computer>();
    foreach (var c in visitedComputers)
    {
        set.Add(c);
        if (!IsSetConnected(set))
            set.Remove(c);
    }

    return set;

    bool LinkExists(Computer a, Computer b) => links.Any(l => l.LinkIsBetween(a, b));

    bool IsSetConnected(HashSet<Computer> testSet)
    {
        var result = true;
        foreach (var c in testSet)
        {
            result &= testSet.All(d => d == c || LinkExists(c, d));
            if (!result)
                return false;
        }

        return result;
    }
}