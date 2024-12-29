using System.Text;
using System.Text.Json;

namespace Day7;

public enum Operator
{
	Plus,
	Multiply,
	Concatenate
}

public record Equation(List<long> operands, long result)
{
	public List<long> Operands { get; init; } = operands;
	public long Result { get; init; } = result;

	public bool CanBeSolved(bool includeConcatenation)
	{
		return CanBeSolved(includeConcatenation, Operands, Result);
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.Append($"{Result}:");
		foreach (var op in Operands)
			sb.Append($" {op}");

		return sb.ToString();
	}

	private bool CanBeSolved(bool includeConcatenation, List<long> operands, long? result)
	{
		if (result == null)
			return false;
		if (operands.Count == 1)
			return operands[0] == result;

		return GetPossibleMoves(includeConcatenation, operands, result).Any(t => CanBeSolved(includeConcatenation, t.remainingOps, t.remainingResult));
	}

	private IEnumerable<(long left, Operator op, List<long> remainingOps, long? remainingResult)> GetPossibleMoves(bool includeConcatenation, List<long> operands, long? result)
	{
		if (result == null)
			yield break;

		foreach (Operator o in Enum.GetValues<Operator>())
		{
			if (!includeConcatenation && o == Operator.Concatenate)
				continue;

			yield return (operands[^1], o, operands[..^1], ApplyOperator(operands[^1], o, result.Value));
		}
	}

	private long? ApplyOperator(long operand, Operator op, long result)
	{
		var resultString = result.ToString();
		return op switch
		{
			Operator.Plus => result - operand,
			Operator.Multiply => result % operand == 0 ? result / operand : null,
			Operator.Concatenate => result >= 0 && result != operand && resultString.EndsWith(operand.ToString()) ? long.Parse(resultString[..^operand.ToString().Length]) : null,
			_ => throw new InvalidOperationException("Unknown operator")
		};
	}
}
