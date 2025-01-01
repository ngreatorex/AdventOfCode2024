using Day12;
using Serilog;

using var log = new LoggerConfiguration()
    .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .CreateLogger();

var map = new Map(log, "Input.txt");

log.Information("Loaded {Rows} x {Cols} map {Map}", map.Height, map.Width, map);
log.Information("Calculated non-discounted costs: {@DiscountedCosts}", map.Costs);
log.Information("Calculated discounted costs: {@DiscountedCosts}", map.DiscountedCosts);

log.Information("Map has total cost: {TotalCost}",  map.TotalCost);
log.Information("Map has total discounted cost {DiscountedCost}", map.TotalDiscountedCost);
