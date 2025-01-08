using Serilog;
using System.Text;

namespace Day21;

public class Keypad(ILogger logger, int keypadNumber, Key?[,] keys, Key start)
{
    protected ILogger Logger { get; } = logger;
    public int KeypadNumber { get; } = keypadNumber;
    public Key?[,] Keys { get; } = keys;
    public Key Current { get; protected set; } = start;
    public int Height => Keys.GetLength(0);
    public int Width => Keys.GetLength(1);

    public Dictionary<char, Key> KeysByChar => Keys.Cast<Key?>().Where(k => k != null).Cast<Key>().ToDictionary(k => k!.Value, k => k);
    public List<Key> SequenceToKeys(string sequence) => sequence.Select(c => KeysByChar[c]).ToList();
    public IEnumerable<Key> MovesToKeys(IEnumerable<Move> moves) => moves.Select(m => KeysByChar[(char)m]).ToList();
    public IEnumerable<Move> KeyPairToMoves(Key from, Key to)
    {
        var movesRight = to.X - from.X;
        var movesDown = to.Y - from.Y;
        var verticalMoves = new List<Move>(Math.Abs(movesDown));
        var horizontalMoves = new List<Move>(Math.Abs(movesRight));


        if (movesDown >= 0)
        {
            verticalMoves.AddRange(Enumerable.Repeat(Move.South, movesDown));
        }
        if (movesDown < 0)
        {
            verticalMoves.AddRange(Enumerable.Repeat(Move.North, -movesDown));
        }
        if (movesRight < 0)
        {
            horizontalMoves.AddRange(Enumerable.Repeat(Move.West, -movesRight));
        }
        if (movesRight >= 0)
        {
            horizontalMoves.AddRange(Enumerable.Repeat(Move.East, movesRight));
        }

        var selectedCombo = FindMoveThatDoesntPassOverEmptySpace(from, [
            verticalMoves.Concat(horizontalMoves).Append(Move.Push),
            horizontalMoves.Concat(verticalMoves).Append(Move.Push)
        ]);

        Logger.Debug("Moves on Keypad {Keypad} from {From} to {To}: {Moves}", KeypadNumber, from, to, selectedCombo);
        return selectedCombo.ToList();
    }

    private IEnumerable<Move> FindMoveThatDoesntPassOverEmptySpace(Key start, IEnumerable<IEnumerable<Move>> moves)
    {
        foreach (var possibleMoves in moves)
        {
            var y = start.Y;
            var x = start.X;
            var isInvalid = false;

            foreach (var move in possibleMoves)
            {
                y += movesToCoords[move].y;
                x += movesToCoords[move].x;

                if (Keys[y, x] == null)
                {
                    isInvalid = true;
                    break;
                }
            }

            if (!isInvalid)
            {
                return possibleMoves;
            }
        }

        throw new InvalidOperationException("No valid move combination found");
    }
    private static readonly Dictionary<Move, (int y, int x)> movesToCoords = new()
    {
        { Move.North, (-1, 0) },
        { Move.East, (0, 1) },
        { Move.South, (1, 0) },
        { Move.West, (0, -1) },
        { Move.Push, (0, 0) }
    };

    public IEnumerable<(Key key, IEnumerable<(Key key, IEnumerable<Move> moves)> moves)> MovesToMoves(IEnumerable<(Key key, IEnumerable<Move> moves)> m)
    {
        var moves = m.Select(n => n.moves).Select(MovesToKeys)
             .Aggregate((Current, []), ((Key last, IEnumerable<(Key, IEnumerable<Move>)> moves) acc, Key key) =>
                 (key, acc.moves.Append((key, KeyPairToMoves(acc.last, key)))
             .ToList()));

        Logger.Verbose("Sequence to moves returned {Tuple}", moves);
        //Current = moves.Item1;

        return moves.Item2;
    }

    public IEnumerable<(Key key, IEnumerable<Move> moves)> SequenceToMoves(string sequence)
    {
        var moves = SequenceToKeys(sequence)
            .Aggregate((Current, []), ((Key last, List<(Key, IEnumerable<Move>)> moves) acc, Key key) =>
                (key, acc.moves.Append((key, KeyPairToMoves(acc.last, key)))
            .ToList()));

        Logger.Verbose("Sequence to moves returned {Tuple}", moves.Item2);
        Current = moves.Item1;

        return moves.Item2;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Keys.GetLength(0); i++)
        {
            if (i == 0)
                AppendLineSeparator(i);

            for (var j = 0; j < Keys.GetLength(1); j++)
            {
                if (Keys[i, j] == null)
                {
                    sb.Append("    ");
                }
                else
                {
                    sb.Append($"| {Keys[i, j]!.Value}");
                    if (Keys[i, j] == Current)
                    {
                        sb.Append('*');
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                } 
            }
            sb.AppendLine("|");

            AppendLineSeparator(i, i+1);
        }

        return sb.ToString();

        void AppendLineSeparator(params int[] lines)
        {
            for (var j = 0; j < Keys.GetLength(1); j++)
            {
                if (lines.Where(IsValidLineIndex).Any(i => Keys[i, j] != null))
                {
                    sb.Append("+---");
                }
                else
                {
                    sb.Append("    ");
                }
            }
            sb.AppendLine("+");
        }

        bool IsValidLineIndex(int index) => index >= 0 && index < Keys.GetLength(0);
    }
}
