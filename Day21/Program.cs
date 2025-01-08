using Day21;
using Serilog;

var logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateLogger();

var puzzle = new Puzzle(logger);
var fileName = "Sample.txt";

long totalComplexity = 0;
await foreach (var line in File.ReadLinesAsync(fileName))
{
    var complexity = puzzle.GetComplexityForCode(line);
    logger.Information("Complexity for code {Code} is {Complexity}", line, complexity);
    totalComplexity += complexity;
}

logger.Information("Total complexity is {TotalComplexity}", totalComplexity);
