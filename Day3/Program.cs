

using System.Collections.Immutable;

var input = File.ReadAllText("input.txt");

PartOne();
PartTwo();
void PartOne() =>
    Console.WriteLine(GetBanks()
        .Select(x => LargestJoltage(x, 2))
        .Sum());

void PartTwo() =>
    Console.WriteLine(GetBanks()
        .Select(x => LargestJoltage(x, 12))
        .ToImmutableArray()
        .Sum());

long LargestJoltage(string bank, int length)
{
    var lastIndexOfFirstDigit = length - 1;
    var firstDigit = bank[..^lastIndexOfFirstDigit].Max();
    var candidates = new Queue<(int Index, string Value, int MissingDigits)>();
    foreach (var (digit, index) in (bank[..^lastIndexOfFirstDigit].Zip(Enumerable.Range(0, bank.Length - lastIndexOfFirstDigit))))
    {
        if (digit == firstDigit)
        {
            candidates.Enqueue((index, firstDigit.ToString(), length - 1));
        }
    }

    List<string> results = new List<string>();
    while (candidates.Count != 0)
    {
        var (index, value, missingDigits) = candidates.Dequeue();

        if (missingDigits == 0)
        {
            results.Add(value);
        }
        else
        {
            var largestValue = GetLargestValue(bank, index + 1, missingDigits);
            candidates.Enqueue((largestValue.Index, value + largestValue.Digit, missingDigits - 1));
        }
    }

    var largestJoltageAsString = results.MaxBy(long.Parse)!;
    return long.Parse(largestJoltageAsString);
}

(int Index, char Digit) GetLargestValue(string bank, int indexOffSet, int missingValues)
{
    (int Index, char Digit) result = (0, '0');
    var subBank = bank[indexOffSet..];
    var lastIndexOfDigit = missingValues - 1;
    foreach (var (digit, index) in (subBank.Zip(Enumerable.Range(0, subBank.Length - lastIndexOfDigit))))
    {
        if (digit > result.Digit)
        {
            result.Index = index + indexOffSet;
            result.Digit = digit;
        }
    }
    return result;
}

ImmutableArray<string> GetBanks() =>
    [
        ..input.Split("\r\n")
    ];