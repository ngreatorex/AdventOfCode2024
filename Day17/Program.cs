using Day17;
using Serilog;

using var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var computer = new Computer(logger, "Input.txt");

logger.Information("Initial state: {Computer}", computer);
computer.Run();
logger.Information("After running program: {Computer}", computer);

var correctedRegisterA = computer.FindCorrectedRegisterA();

logger.Information("Program: {Program}", string.Join(",", computer.Program.Select(op => $"{(int)op.Opcode},{op.Operand}")));
logger.Information("Output: {Output}", string.Join(",", computer.Output));
logger.Information("Corrected register A value is {CorrectedRegisterA}", correctedRegisterA);