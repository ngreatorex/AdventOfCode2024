using Day15;
using Serilog;

var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var fileName = "Input.txt";
foreach (var wideMode in new bool[] { false, true })
{
    var warehouse = new Warehouse(logger, fileName, wideMode);

    logger.Information("Initial (wide mode is {WideMode}) {State}", wideMode, warehouse);
    warehouse.CompleteMoves();
    logger.Information("Final (wide mode is {WideMode}) {State}", wideMode, warehouse);
    logger.Information("Box GPS sum (wide mode is {WideMode}) is {Sum}", wideMode, warehouse.BoxesGPSSum);

}
