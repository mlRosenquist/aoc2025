

using System.Collections.Immutable;

var input = File.ReadAllText("input.txt");

PartTwo();
void PartOne()
{
    var banks = GetBanks();
    var totalJoltage = banks
        .Select(x => LargestJoltage(x, 2))
        .Sum();
    Console.WriteLine(totalJoltage);
}

void PartTwo()
{
    var banks = GetBanks();
    var enumerable = banks
        .Select(x => LargestJoltage(x, 12))
        .ToImmutableArray();
    
    var totalJoltage = enumerable
        .Sum();
    Console.WriteLine(totalJoltage);
}

long LargestJoltage(string bank, int length)
{
    var firstDigit = bank[..^(length - 1)].MaxBy(x => int.Parse(x.ToString()));
    var candidates = new Queue<(int Index, string Value, int MissingValues)>();
    foreach (var (c, index) in (bank[..^(length - 1)].Zip(Enumerable.Range(0, bank.Length-(length - 1)))))
    {
        if (c == firstDigit)
        {
            candidates.Enqueue((index, firstDigit.ToString(), length - 1));
        }
    }

    List<string> results = new List<string>();
    while (candidates.Count != 0)
    {
        var (index, value, missingValues) = candidates.Dequeue();

        if (missingValues == 0)
        {
            results.Add(value);
        }
        else
        {
            var largestValue = GetLargestValue(bank, index + 1, missingValues);
            candidates.Enqueue((largestValue.Index, value + largestValue.Value, missingValues - 1));
        }
    }

    return long.Parse(results.MaxBy(long.Parse)!);
}

(int Index, int Value) GetLargestValue(string bank, int indexOffSet, int missingValues)
{
    var result = (0, 0);
    var subBank = bank[indexOffSet..];
    foreach (var (c, index) in (subBank.Zip(Enumerable.Range(0, subBank.Length))))
    {
        if (subBank.Length - index < missingValues)
        {
            continue;
        }
        
        var value = int.Parse(c.ToString());

        if (value > result.Item2)
        {
            result.Item1 = index + indexOffSet;
            result.Item2 = value;
        }
    }
    return result;
}

ImmutableArray<string> GetBanks() =>
[
    ..input.Split("\r\n")
];