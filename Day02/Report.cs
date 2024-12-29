public class Report
{
    public required List<int> Levels { get; set; }

    public bool AreLevelsIncreasing => Levels.OrderBy(n => n).SequenceEqual(Levels);
    public bool AreLevelsDecreasing => Levels.OrderByDescending(n => n).SequenceEqual(Levels);
    public bool AreStepsSafe => Levels.Zip(Levels.Skip(1), (prev, current) => Math.Abs(current - prev) >= 1 && Math.Abs(current - prev) <= 3).All(x => x);

    public bool IsSafe => (AreLevelsDecreasing || AreLevelsIncreasing) && AreStepsSafe;

    public bool IsSafeWithProblemDampener => IsSafe || Enumerable.Range(0, Levels.Count).Any(indexToRemove => 
    {
        var newLevels = new List<int>(Levels);
        newLevels.RemoveAt(indexToRemove);

        var newReport = new Report() { Levels = newLevels };

        Console.WriteLine($"Original report: {string.Join(' ', Levels)}, new: {string.Join(' ', newReport.Levels)}, now safe? {newReport.IsSafe}");
        return newReport.IsSafe;
    });
}
