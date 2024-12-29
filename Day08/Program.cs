using Day08;

var fileName = "Input.txt";

var part1Map = new Map(fileName, false);
part1Map.Print();
Console.WriteLine($"Map contains {part1Map.AntinodeCount} antinodes");

var part2Map = new Map(fileName, true);
Console.WriteLine($"Map contains {part2Map.AntinodeCount} antinodes when including resonance");