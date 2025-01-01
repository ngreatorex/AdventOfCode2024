using Day13;
using Serilog;

using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var lines = File.ReadAllLines("Input.txt");
var machines = new List<ClawMachine>();

for (var i=0; i < lines.Length; i+=4)
{
    machines.Add(new ClawMachine(lines[i..(i + 3)]));
}

RunSolution(false);
RunSolution(true);

return;

void RunSolution(bool useCorrectedPrizeLocation)
{
    long totalCost = 0;
    foreach (var machine in machines)
    {
        var solution = machine.Solve(useCorrectedPrizeLocation);
        var cost = solution?.Keys.Select(b => (long)solution[b] * ClawMachine.Costs[b]).Sum();
        if (cost.HasValue)
            totalCost += cost.Value;

        log.Information("Machine {@Machine} has {Corrected} solution {@Solution} with cost {Cost}", machine, useCorrectedPrizeLocation ? "corrected" : "uncorrected", solution, cost);
    }
    log.Information("Total cost is {TotalCost}", totalCost);
}