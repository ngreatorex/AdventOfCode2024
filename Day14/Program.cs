
using Day14;
using Serilog;

using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var floor = new Floor(log, "Input.txt", 101, 103);
//var floor = new Floor(log, "Sample1.txt", 11, 7);
//var floor = new Floor(log, "Sample2.txt", 11, 7);

log.Information("Loaded {RobotCount} robots", floor.Robots.Count);

var i = 100;
floor.Move(i);
log.Information("After {Sec} seconds, {State}", i, floor);
log.Information("After {Sec} seconds, safety factor is {SafetyFactor}", i, floor.SafetyFactor);

var xmasMoves = floor.MoveUntilPossibleXmasTree();
log.Information("Moved total of {Sec} seconds to get to {State}", i + xmasMoves, floor);