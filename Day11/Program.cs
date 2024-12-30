using Day11;

var iterations = 100;
var memoryStones = new Stones(StoneStorageFactory.List, "Input.txt");
var stones = new Stones(StoneStorageFactory.Dictionary, "Input.txt");

Console.WriteLine("Initial state:");
Console.WriteLine(stones);

const string correct = "correct";
const string incorrect = "INCORRECT";

for (var i = 1; i <= iterations; i++)
{
    stones.Blink();

    if (i < 10)
    { 
        memoryStones.Blink();

        Console.WriteLine($"After {i} blinks: {stones.Values.Count:N0} stones (memory version has {memoryStones.Values.Count:N0} stones, {(memoryStones.Values.Count == stones.Values.Count ? correct : incorrect)})");
        Console.WriteLine(stones);
        Console.WriteLine();
    }
    else if (i == 25 || i == 75 || i == iterations)
    {
        Console.WriteLine($"After {i} blinks: {stones.Values.Count:N0} stones ({stones.Values.DistinctStones:N0} distinct stone values)");
    }
}