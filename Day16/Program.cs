using Day16;
using Serilog;

var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var fileName = "Input.txt";
var maze = await Maze.OpenMaze(logger, fileName);
maze.Solve();