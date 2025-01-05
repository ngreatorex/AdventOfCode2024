using Day19;
using Serilog;
using System.Collections.Concurrent;

using var logger = new LoggerConfiguration()
    .Destructure.AsDictionary<ConcurrentDictionary<string,long>>()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var puzzle = new Puzzle(logger, "Input.txt");
var solutions = puzzle.Solve(false);

logger.Information("Designs with solutions: {@Solutions}", solutions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value > 0));
logger.Information("Found {Count} designs with solutions", solutions.Where(kvp => kvp.Value > 0).Count());

var allSolutions = puzzle.Solve(true);
logger.Information("Solution count per design: {@Solutions}", allSolutions);
logger.Information("Found {Count} possible solutions", allSolutions.Sum(kvp => kvp.Value));