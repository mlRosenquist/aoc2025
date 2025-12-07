// See https://aka.ms/new-console-template for more information


using System.Collections.Immutable;

////var amountFreshIngredients = 6;
////PartOne();
PartTwo();
////void PartOne()
////{
////    var input = File.ReadAllText("input.txt");
////    var fresh = FreshIngredients(input);
////    var available = AvailableIngredients(input);

////    var amountFresh = available
////        .Count(IsFresh);
    
////    Console.WriteLine(amountFresh);
////    bool IsFresh(long ingredient) =>
////        fresh.Any(x => x.Start <= ingredient && ingredient <= x.End);
////}

void PartTwo()
{
    var input = File.ReadAllText("input.txt");
    var fresh = FreshIngredients(input);

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

        var coveringRange = (start, end);

        if (lowerOverlappingRanges.Any())
        {
            var min = lowerOverlappingRanges.Min(x => x.Start);
            coveringRange.start = min;
        }

        if (higherOverlappingRanges.Any())
        {
            var max = higherOverlappingRanges.Max(x => x.End);
            coveringRange.end = max;
        }

        if (start < coveringRange.start)
        {
            coveringRange.start = start;
        }

        if (end > coveringRange.end)
        {
            coveringRange.end = end;
        }

        foreach (var oneOfOverlappingRange in handledRanges.Where(x => x.Start == start + 1).ToImmutableArray())
        {
            handledRanges.Add((start, oneOfOverlappingRange.End));
        }

        foreach (var oneOfOverlappingRange in handledRanges.Where(x => x.End == start - 1).ToImmutableArray())
        {
            handledRanges.Add((oneOfOverlappingRange.Start, end));
        }

        handledRanges.Add(coveringRange);
        foreach (var range in handledRanges.ToImmutableArray())
        {
            if (handledRanges.Any(x => 
                    x.Start <= range.Start && x.End >= range.End &&
                    (x.Start != range.Start || x.End != range.End)))
            {
                handledRanges.Remove(range);
            }
        }
        Console.WriteLine($"({start},{end}):");
        foreach (var range in handledRanges)
        {
            Console.WriteLine($"({range.Start},{range.End})");
        }
        Console.WriteLine($"---");
    }

    var result = handledRanges
        .Sum(x => x.End - x.Start + 1);
    Console.WriteLine(result);
}

ImmutableArray<(long Start, long End)> FreshIngredients(string input) =>
    input.Split("\r\n")//[..amountFreshIngredients]
        .Select(x => x.Split("-"))
        .Select(x => (long.Parse(x[0].Trim()), long.Parse(x[1].Trim())))
        .ToImmutableArray();

////ImmutableArray<long> AvailableIngredients(string input) =>
////    [
////        ..input.Split("\r\n")[(amountFreshIngredients + 1)..]
////            .Select(long.Parse)
////    ];
