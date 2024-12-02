var lines = File.ReadLinesAsync("Day2.txt");  
var data = new List<Report>();

await foreach (var line in lines)
{
    var report = new Report()
    {
        Levels = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList()
    };
    data.Add(report);
}

Console.WriteLine($"Total reports: {data.Count}, safe reports: {data.Count(r => r.IsSafe)}.");
Console.WriteLine($"Reports that are safe using problem dampener: {data.Count(r => r.IsSafeWithProblemDampener)}");