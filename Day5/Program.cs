// See https://aka.ms/new-console-template for more information


using System.Collections.Immutable;

var amountFreshIngredients = 10;
PartOne();
PartTwo();
void PartOne()
{
    var input = File.ReadAllText("input.txt");
    var fresh = FreshIngredients(input);
    var available = AvailableIngredients(input);

    var amountFresh = available
        .Count(IsFresh);
    
    Console.WriteLine(amountFresh);
    bool IsFresh(long ingredient) =>
        fresh.Any(x => x.Start <= ingredient && ingredient <= x.End);
}

void PartTwo()
{
    var input = File.ReadAllText("input.txt");
    var fresh = FreshIngredients(input);

    var result = 0L;
    var handledRanges = new List<(long Start, long End)>();

    foreach ((var start, var end) in fresh)
    {
        var completelyOverlappingRange = handledRanges.Where(x => x.Start <= start && x.End >= end);
        if (completelyOverlappingRange.Any())
        {
            continue;
        }
        
        var lowerOverlappingRanges = handledRanges
            .Where(x => x.Start <= start && x.End <= end)
            .ToImmutableArray();
        var higherOverlappingRanges = handledRanges
            .Where(x => x.Start >= start && x.End >= end)
            .ToImmutableArray();

        var resultRange = (start, end);
        if (lowerOverlappingRanges.Any())
        {
            foreach (var lowerOverlappingRange in lowerOverlappingRanges)
            {
                if (lowerOverlappingRange.End >= resultRange.start)
                {
                    resultRange.start = lowerOverlappingRange.End + 1;
                }
            }
        }
        
        if (higherOverlappingRanges.Any())
        {
            foreach (var higherOverlappingRange in higherOverlappingRanges)
            {
                if (higherOverlappingRange.Start <= resultRange.end)
                {
                    resultRange.end = higherOverlappingRange.Start - 1;
                }
            }
        }

        var rangeDiff = resultRange.end - resultRange.start;
        if (rangeDiff == 0)
        {
            var lowerEnd = lowerOverlappingRanges.Max(x => x.End);
            var higherStart = higherOverlappingRanges.Min(x => x.Start);
            if (lowerEnd >= higherStart)
            {
                continue;
            }
        }
        
        result += rangeDiff + 1;
        handledRanges.Add((start, end));
    }
    Console.WriteLine(result);
}

ImmutableArray<(long Start, long End)> FreshIngredients(string input) =>
    input.Split("\r\n")[..amountFreshIngredients]
        .Select(x => x.Split("-"))
        .Select(x => (long.Parse(x[0].Trim()), long.Parse(x[1].Trim())))
        .ToImmutableArray();

ImmutableArray<long> AvailableIngredients(string input) =>
    [
        ..input.Split("\r\n")[(amountFreshIngredients + 1)..]
            .Select(long.Parse)
    ];
