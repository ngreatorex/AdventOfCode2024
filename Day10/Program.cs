
using Day10;

var map = new TopoMap("Input.txt");
Console.WriteLine(map);
Console.WriteLine($"Score: {map.GetScore()}, Rating: {map.GetRatingSum()}");