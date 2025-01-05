using Serilog;
using System.Collections.Concurrent;

namespace Day19;

public class Puzzle
{
    private readonly ILogger logger;

    private readonly ConcurrentDictionary<string, long> solutionCache = new();
    private long cacheHits = 0;
    private long cacheMisses = 0;

    public List<string> Patterns { get; } = [];
    public List<string> Designs { get; } = [];

    public Puzzle(ILogger logger, string fileName)
    {
        this.logger = logger;
        var lines = File.ReadAllLines(fileName);

        if (lines.Length < 3)
            throw new ArgumentException("Input does not have enough lines", nameof(fileName));

        Patterns.AddRange(lines[0].Split(", "));

        for (var i=2; i < lines.Length; i++)
            Designs.Add(lines[i]);

        logger.Information("Loaded puzzle with patterns {@Patterns} and designs {@Designs}", Patterns, Designs);
    }

    public ConcurrentDictionary<string, long> Solve(bool returnAllSolutions)
    {
        var result = new ConcurrentDictionary<string, long>();
        var processedCount = 0;

        Patterns.Sort((x, y) => y.Length - x.Length);

        solutionCache.Clear();
        cacheHits = cacheMisses = 0;

        foreach (var design in Designs)
        {
            result[design] = Solve(design, returnAllSolutions);
            Interlocked.Increment(ref processedCount);

            logger.Debug("Processed {Count}/{Total} designs. Last: {Design}, Cache stats: {CacheHits}/{CacheTotal} ({CacheHitPercentage}%)", processedCount, Designs.Count, design, cacheHits, cacheHits + cacheMisses, 100 * cacheHits / (cacheHits + cacheMisses));
        }

        return result;
    }

    private long Solve(string design, bool returnAllSolutions)
    {
        if (design == string.Empty)
        {
            return 1;
        }

        if (solutionCache.TryGetValue(design, out long value))
        {
            Interlocked.Increment(ref cacheHits);
            logger.Verbose("Using solution cache of {Count} solutions for design {Design}", value, design);

            return value;
        }

        Interlocked.Increment(ref cacheMisses);

        long matches = 0;

        foreach (var pattern in Patterns.Where(p => p.Length <= design.Length))
        {
            if (design.StartsWith(pattern))
            {
                matches += Solve(design[pattern.Length..], returnAllSolutions);

                if (!returnAllSolutions)
                    return matches;
            }
        }

        logger.Verbose("Adding {Count} solutions to the cache for design {Design}", matches, design);
        solutionCache[design] = matches;

        return matches;
    }
}
