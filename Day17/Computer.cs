using Serilog;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day17;

public partial class Computer
{
    private readonly ILogger logger;

    private long InitialB { get; }
    private long InitialC { get; }

    public long RegisterA { get; private set; }
    public long RegisterB { get; private set; }
    public long RegisterC { get; private set; }

    public int InstructionPointer { get; private set; }

    public List<Operation> Program { get; } = [];
    public List<int> Output { get; } = [];

    public Computer(ILogger logger, string fileName)
    {
        var lines = File.ReadAllLines(fileName);

        if (lines.Length != 5)
            throw new ArgumentException("File contains invalid program", nameof(fileName));

        var matches = Enumerable.Range(0, 3).Select(i => RegisterRegex().Match(lines[i])).ToArray();

        if (matches.Any(m => !m.Success))
            throw new ArgumentException("File contains invalid program", nameof(fileName));

        if (matches[0].Groups[1].Value != "A")
            throw new ArgumentException("First line of program is not setting register A", nameof(fileName));
        RegisterA = int.Parse(matches[0].Groups[2].Value);
        if (matches[1].Groups[1].Value != "B")
            throw new ArgumentException("Second line of program is not setting register B", nameof(fileName));
        RegisterB = InitialB = int.Parse(matches[1].Groups[2].Value);
        if (matches[2].Groups[1].Value != "C")
            throw new ArgumentException("Third line of program is not setting register C", nameof(fileName));
        RegisterC = InitialC = int.Parse(matches[2].Groups[2].Value);

        var programMatch = ProgramRegex().Match(lines[4]);
        if (!programMatch.Success)
            throw new ArgumentException("File contains invalid program", nameof(fileName));

        Program.Add(new Operation((Opcode)int.Parse(programMatch.Groups[1].Value), int.Parse(programMatch.Groups[2].Value)));
        for (var i = 0; i < programMatch.Groups[4].Captures.Count; i++)
        {
            Program.Add(new Operation((Opcode)int.Parse(programMatch.Groups[4].Captures[i].Value), int.Parse(programMatch.Groups[5].Captures[i].Value)));
        }

        this.logger = logger;
    }

    public override string ToString()
    {
        return $"{{Computer IP={InstructionPointer} A={RegisterA} B={RegisterB} C={RegisterC} Output={string.Join(",", Output)} Program={string.Join(",", Program)}}}";
    }

    private void Reset(long A)
    {
        RegisterA = A;
        RegisterB = InitialB;
        RegisterC = InitialC;
        InstructionPointer = 0;
        Output.Clear();
    }

    public long FindCorrectedRegisterA()
    {
        var originalProg = Program.SelectMany<Operation, int>(op => [(int)op.Opcode, op.Operand]).ToList();
        var currentGuess = 0L;

        for (var outputIndex = 0; outputIndex < originalProg.Count; outputIndex++)
        {
            for (currentGuess <<= 3; 
                currentGuess < long.MaxValue && !(Output.Count == outputIndex + 1 && EndsWith(originalProg, Output)); 
                currentGuess++)
            {
                logger.Verbose("Testing index {CurrentGuess}", currentGuess);
                Reset(currentGuess);
                Run();
            }
            currentGuess--;
        }

        return currentGuess;

        static bool EndsWith<T>(List<T> a, List<T> b) => a[^b.Count..].SequenceEqual(b);
    }

    public void Run()
    {
        while (InstructionPointer >= 0 && InstructionPointer < Program.Count * 2)
        {
            if (Operate(Program[InstructionPointer / 2]))
                InstructionPointer += 2;
        }
    }

    private bool Operate(Operation op)
    {
        var advanceIP = true;
        switch (op.Opcode)
        {
            case Opcode.Adv:
                RegisterA = DoDv(op);
                break;

            case Opcode.Bxl:
                RegisterB ^= op.Operand;
                break;

            case Opcode.Bst:
                RegisterB = ParseComboOperand(op) % 8;
                break;

            case Opcode.Jnz:
                if (RegisterA == 0)
                    break;
                InstructionPointer = op.Operand;
                advanceIP = false;
                break;

            case Opcode.Bxc:
                RegisterB ^= RegisterC;
                break;

            case Opcode.Out:
                var output = (int)(ParseComboOperand(op) % 8);
                Debug.Assert(output >= 0);
                Output.Add(output);
                break;

            case Opcode.Bdv:
                RegisterB = DoDv(op);
                break;

            case Opcode.Cdv:
                RegisterC = DoDv(op);
                break;

            default:
                throw new InvalidOperationException("Unknown opcode");
        }
        logger.Verbose("After operation {Operation}, state is: {Computer}", op, this);

        return advanceIP;

        long DoDv(Operation op) => RegisterA >> (int)ParseComboOperand(op);
        long ParseComboOperand(Operation op) => op.Operand switch
        {
            >= 0 and <= 3 => op.Operand,
            4 => RegisterA,
            5 => RegisterB,
            6 => RegisterC,
            _ => throw new InvalidOperationException("Unknown combo operand value")
        };
    }

    [GeneratedRegex(@"^Register ([ABC]): (\d+)$")]
    private static partial Regex RegisterRegex();

    [GeneratedRegex(@"^Program: (\d+),(\d+)(,(\d+),(\d+))*$")]
    private static partial Regex ProgramRegex();
}


