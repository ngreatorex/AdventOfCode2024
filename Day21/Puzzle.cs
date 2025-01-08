using Serilog;

namespace Day21;

public class Puzzle
{
    public List<Keypad> Keypads { get; } = [];

    internal ILogger Logger { get; }

    public Puzzle(ILogger logger)
    {
        Logger = logger;

        Keypads.AddRange(GetNumericalKeypad(1), GetDirectionalKeypad(2), GetDirectionalKeypad(3));
    }

    public long GetComplexityForCode(string code)
    {
        var currentMoves = Keypads[0].SequenceToMoves(code);

        for (var i = 0; i < Keypads.Count; i++)
        {
            currentMoves = currentMoves.Select(m => Keypads[i].MovesToMoves(m.moves))
            Logger.Debug("Input sequence to enter code {Code} on keypad {Keypad} is {Moves} (length {Length})", code, i + 1, currentMoves, currentMoves.Length);
        }

        var numbers = int.Parse(string.Join("", code.Where(char.IsDigit)));

        Logger.Debug("Input sequence to enter code {Code} is {Moves}", code, currentMoves);
        Logger.Debug("Code {Code} has complexity {Complexity} ({Length} * {Number})", code, currentMoves.Length * numbers, currentMoves.Length, numbers);
        return currentMoves.Length * numbers;
    }

    private string MovesToString(IEnumerable<(Key, IEnumerable<Move>)> moves) =>
        string.Join("", MovesToChar(moves).Select(t => string.Join("", t.Item2)));

    private IEnumerable<(Key, IEnumerable<char>)> MovesToChar(IEnumerable<(Key, IEnumerable<Move>)> moves) => 
        moves.Select(t => (t.Item1, t.Item2.Select(m => (char)m)));

    private Keypad GetNumericalKeypad(int number)
    {
        var keys = new Key[4, 3];

        keys[0, 0] = new Key('7', 0, 0);
        keys[0, 1] = new Key('8', 0, 1);
        keys[0, 2] = new Key('9', 0, 2);
        keys[1, 0] = new Key('4', 1, 0);
        keys[1, 1] = new Key('5', 1, 1);
        keys[1, 2] = new Key('6', 1, 2);
        keys[2, 0] = new Key('1', 2, 0);
        keys[2, 1] = new Key('2', 2, 1);
        keys[2, 2] = new Key('3', 2, 2);
        keys[3, 1] = new Key('0', 3, 1);
        keys[3, 2] = new Key('A', 3, 2);

        return new Keypad(Logger, number, keys, keys[3, 2]);
    }

    private Keypad GetDirectionalKeypad(int number)
    {
        var keys = new Key[2, 3];

        keys[0, 1] = new Key('^', 0, 1);
        keys[0, 2] = new Key('A', 0, 2);
        keys[1, 0] = new Key('<', 1, 0);
        keys[1, 1] = new Key('v', 1, 1);
        keys[1, 2] = new Key('>', 1, 2);
 
        return new Keypad(Logger, number, keys, keys[0, 2]);
    }
}
