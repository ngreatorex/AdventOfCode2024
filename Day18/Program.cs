
using System.Text.RegularExpressions;
using Day18;

var time = 1024; //12 or 1024
var lines = File.ReadLinesAsync("Input.txt"); //Sample.txt or Input.txt
var regex = new Regex(@"^([0-9]+),([0-9]+)$");

var bytesToFall = new List<(int y, int x)>();

await foreach (var line in lines)
{
    var match = regex.Match(line);
    if (match.Success)
    {
        var x = int.Parse(match.Groups[1].Value);
        var y = int.Parse(match.Groups[2].Value);

        bytesToFall.Add((y,x));
    }
}

var width = bytesToFall.Max(b => b.x) + 1;
var height = bytesToFall.Max(b => b.y) + 1;

var memorySpace = new Coordinate[height,width];

for (var y=0; y < memorySpace.GetLength(0); y++)
{
	for (var x=0; x < memorySpace.GetLength(1); x++)
	{
		memorySpace[y,x] = new Coordinate();
	}
}

Console.WriteLine($"We have a memory space that is {width} x {height} and {bytesToFall.Count} bytes to fall.");

for (var b = 0; b < time && b < bytesToFall.Count; b++)
{
	var tuple = bytesToFall[b];
	memorySpace[tuple.y,tuple.x].IsCorrupted = true;
}

var end = FindShortestPath();

var pathLength = -1;
var current = end;

while (current != null)
{
	pathLength++;
	current.IsOnSolutionPath = true;
	current = current.Parent;
}
PrintMemorySpace();

if (end != null)
	Console.WriteLine($"Found solution with length {pathLength}");
else
	Console.WriteLine("No solution found");


Coordinate? endOfPath = memorySpace[height-1,width-1];
var maxB = time-1;
for (var b = time; b < bytesToFall.Count && endOfPath != null; b++)
{
	var tuple = bytesToFall[b];
	memorySpace[tuple.y,tuple.x].IsCorrupted = true;
	endOfPath = FindShortestPath();

	Console.WriteLine($"t={b}, does path exist? {endOfPath != null}");
	if (endOfPath != null)
		maxB = b;
}

Console.WriteLine($"Path ended at time {maxB}. Next byte to fall is {bytesToFall[maxB+1].x},{bytesToFall[maxB+1].y}");

return;

void PrintMemorySpace()
{
	for (var y=0; y < memorySpace.GetLength(0); y++)
	{
		Console.Write($"y={y}: ");
		for (var x=0; x < memorySpace.GetLength(1); x++)
		{
			Console.Write(memorySpace[y,x].ToString());
		}
		Console.WriteLine();
	}
}

IEnumerable<(int y, int x)> GetNextMoves(int y, int x)
{
	if (y > 0)
		yield return (y-1, x);
	if (x > 0)
		yield return (y, x-1);
	if (y < height-1)
		yield return (y+1, x);
	if (x < width-1)
		yield return (y, x+1);
}

Coordinate? FindShortestPath()
{
	var explored = new bool[height,width];
	var q = new Queue<(int y, int x)>();

	explored[0,0] = true;
	q.Enqueue((0,0));

	while (q.Count > 0)
	{
		var n = q.Dequeue();
		if (n.y == height-1 && n.x == width-1)
		{
			return memorySpace[n.y,n.x];
		}
		foreach (var nextMove in GetNextMoves(n.y, n.x).Where(t => memorySpace[t.y,t.x].IsCorrupted == false))
		{
			if (explored[nextMove.y,nextMove.x] == false)
			{
				explored[nextMove.y,nextMove.x] = true;
				memorySpace[nextMove.y,nextMove.x].Parent = memorySpace[n.y,n.x];
				q.Enqueue(nextMove);
			}
		}
	}

	return null;
}
