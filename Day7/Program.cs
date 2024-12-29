
using System.Text.RegularExpressions;
using Day7;

var lines = File.ReadLinesAsync("Input.txt"); //Sample.txt or Input.txt
var regex = new Regex(@"^([0-9]+):( ([0-9]+))+$");

var equations = new List<Equation>();

await foreach (var line in lines)
{
    var match = regex.Match(line);
    if (match.Success)
    {
        var result = long.Parse(match.Groups[1].Value);
        var operands = match.Groups[3].Captures.Select(c => long.Parse(c.Value)).ToList();

        equations.Add(new Equation(operands, result));
    }
}

var correctEqs = equations.Where(e => e.CanBeSolved(false)).ToList();
var correctEqsPart2 = equations.Where(e => e.CanBeSolved(true)).ToList();

Console.WriteLine($"{correctEqs.Count} equations have a solution. Total is {correctEqs.Sum(e => e.Result)}");
Console.WriteLine($"{correctEqsPart2.Count} equations have a solution including concatenation operator. Total is {correctEqsPart2.Sum(e => e.Result)}");

