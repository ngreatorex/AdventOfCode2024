using Day6;
using System.Collections.Concurrent;

var filename = "Day6.txt";
var lab = new Lab(filename);
Console.WriteLine(lab.ToString());

var completion = CompletionStatus.Incomplete;
while ((completion = lab.Advance()) == CompletionStatus.Incomplete);

Console.WriteLine("Finished!");
Console.WriteLine($"Completion status: {completion}. Visited {lab.VisitedSquareCount} positions");

if (completion != CompletionStatus.Loop)
{
    var possibleObstacleCandidates = lab.GetVisitedSquares();
    var possibleObstacles = new ConcurrentBag<(int row, int column)>();

    Parallel.ForEach(possibleObstacleCandidates, (tuple) =>
    {
        var (row, column) = tuple;
        var candidateLab = new Lab(filename);
        candidateLab.AddObstacle(row, column);

        var candidateCompletion = CompletionStatus.Incomplete;
        while ((candidateCompletion = candidateLab.Advance()) == CompletionStatus.Incomplete) ;

        if (candidateCompletion == CompletionStatus.Loop)
            possibleObstacles.Add((row, column));
    });


    Console.WriteLine($"Found {possibleObstacles.Count} positions to add obstacles to create a loop");
}