var lines = File.ReadLinesAsync("Day1.txt");  
var count = 0;
var listOne = new List<int>();
var listTwo = new List<int>();

await foreach (var line in lines)
{
	var nums = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
	if (nums.Length != 2)
		continue;

	listOne.Add(nums[0]);
	listTwo.Add(nums[1]);
	count++;
}

Console.WriteLine($"Number of valid lines: {count}");

listOne.Sort();
listTwo.Sort();

var sum = listOne.Zip(listTwo, (a, b) => Math.Abs(b - a)).Sum();
Console.WriteLine($"Sum of differences: {sum}");

var similarityScore = listOne.Select(a => listTwo.Count(b => b == a) * a).Sum();
Console.WriteLine($"Similarity score: {similarityScore}");
