using Day16;
using Serilog;

var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var fileName = "Sample1.txt";
var maze = await Maze.OpenMaze(logger, fileName);
maze.Solve();