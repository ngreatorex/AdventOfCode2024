using Day9;

var lines = File.ReadLinesAsync("Input.txt");

await foreach (var line in lines)
{
    //var map = new DiskMap(line);
    //Console.WriteLine($"Original input: {map}");

    //map.Fragment();
    //Console.WriteLine($"    Fragmented: {map}");
    //Console.WriteLine($"      Checksum: {map.Checksum}");

    var map2 = new DiskMap(line);
    Console.WriteLine($"Original input: {map2}");

    map2.Defragment();
    Console.WriteLine($"  Defragmented: {map2}");
    Console.WriteLine($"      Checksum: {map2.Checksum}");

    Console.WriteLine();
}

