using System.Collections.Immutable;

var amountFreshIngredients = 194;
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
    var uniqueRanges = GetUniqueRanges(fresh);
    var result = uniqueRanges
        .Sum(x => x.End - x.Start + 1);
    Console.WriteLine(result);
}

ImmutableArray<Range> FreshIngredients(string input) =>
[
    ..input.Split("\r\n")[..amountFreshIngredients]
        .Select(x => x.Split("-"))
        .Select(x => new Range(long.Parse(x[0].Trim()), long.Parse(x[1].Trim())))
];

ImmutableArray<long> AvailableIngredients(string input) =>
[
    ..input.Split("\r\n")[(amountFreshIngredients + 1)..]
        .Select(long.Parse)
];

HashSet<Range> GetUniqueRanges(ImmutableArray<Range> immutableArray)
{
    var hashSet = new HashSet<Range>();
    foreach (var range in immutableArray)
    {
        var lowerOverlappingRanges = hashSet
            .Where(x => 
                (x.Start <= range.Start && x.End <= range.End && x.End >= range.Start) ||
                x.End == range.Start - 1)
            .ToImmutableArray();
        var higherOverlappingRanges = hashSet
            .Where(x => 
                (x.Start >= range.Start && x.End >= range.End && x.Start <= range.End) ||
                x.Start == range.End + 1)
            .ToImmutableArray();

        var newRange = (range.Start, range.End);
        if (lowerOverlappingRanges.Any())
        {
            var min = lowerOverlappingRanges.Min(x => x.Start);
            newRange.Start = min;
        }
        if (higherOverlappingRanges.Any())
        {
            var max = higherOverlappingRanges.Max(x => x.End);
            newRange.End = max;
        }

        hashSet.Add(new(newRange.Start, newRange.End));
        Trim();
    }

    return hashSet;

    void Trim()
    {
        foreach (var current in hashSet.ToImmutableArray())
        {
            if (hashSet.Any(x => 
                    x.Start <= current.Start && x.End >= current.End &&
                    (x.Start != current.Start || x.End != current.End)))
            {
                hashSet.Remove(current);
            }
        }
    }
}

record Range(long Start, long End);
