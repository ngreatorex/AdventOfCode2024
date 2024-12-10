
using Day04;

foreach (var filename in new List<string>() { "Sample.txt", "Input.txt" })
{
    var wordsearch = new Wordsearch(filename);

    Console.WriteLine(filename);
    //Console.WriteLine(wordsearch);

    Console.WriteLine($" Occurrences of XMAS: {wordsearch.CountAllMatches(Day04.MatchType.Xmas)}");
    Console.WriteLine($"Occurrences of X-MAS: {wordsearch.CountAllMatches(Day04.MatchType.MasInAnX)}");
    Console.WriteLine();
}