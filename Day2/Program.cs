using System.Collections.Immutable;

var input = File.ReadAllText("input.txt");
PartOne();
PartTwo();
void PartOne() =>
    Console.WriteLine(
        GetIds()
        .Where(IsRepeatedTwice)
        .Sum());

void PartTwo() =>
    Console.WriteLine(
        GetIds()
        .Where(IsRepeated)
        .ToImmutableArray().Sum());

bool IsRepeatedTwice(long id)
{
    var idAsString = id.ToString();
    var length = idAsString.Length;
    return idAsString[..(length/2)] == idAsString[(length/2)..];
}

bool IsRepeated(long id)
{
    var idAsString = id.ToString();
    var length = idAsString.Length;

    for (int i = 1; i <= length/2; i++)
    {
        if (length % i == 0)
        {
            var splits = Split(idAsString, i).ToImmutableArray();
            var first = splits.First();
            if (splits.All(x => x.Equals(first)))
            {
                return true;
            }
        }
    }

    return false;
}

IEnumerable<string> Split(string id, int chunkSize)
{
    for (var i = chunkSize; i <= id.Length; i+=chunkSize)
    {
        yield return id[(i - chunkSize)..i];
    }
}

ImmutableArray<long> GetIds()
{
    return [
        ..input.Split(',')
            .Select(x => x.Split('-'))
            .Select(x => (long.Parse(x[0]), long.Parse(x[1])))
            .SelectMany(x => GetRange(x.Item1, x.Item2))
    ];
}

IEnumerable<long> GetRange(long start, long end)
{
    for (var i = start; i <= end; i++)
    {
        yield return i;
    }
}
