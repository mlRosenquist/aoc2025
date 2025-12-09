using System.Collections.Immutable;

PartOne();
PartTwo();

void PartOne()
{
    var boxes = GetJunctionBoxes();
    var distances = GetDistances(boxes);
    var connections = MakeConnections(boxes, distances, 10);
    var result = connections
        .OrderBy(x => x.Count)
        .TakeLast(3)
        .Aggregate(1, (i, set) => i * set.Count);
    Console.WriteLine(result);
}

void PartTwo()
{
    var boxes = GetJunctionBoxes();
    var distances = GetDistances(boxes);
    var pair = GetLastPair(boxes, distances);
    Console.WriteLine((long)pair.First.X * (long)pair.Second.X);
}

BoxPair GetLastPair(ImmutableArray<Box> boxes, List<BoxPair> distances)
{
    var circuits = boxes
        .Select(x => new HashSet<Box>([x]))
        .ToList();
    var i = 0;
    while (circuits.Count > 1)
    {
        circuits = MakeConnection(circuits, distances[i]);
        i++;
    }

    return distances[i-1];
}

List<HashSet<Box>> MakeConnections(ImmutableArray<Box> boxes, List<BoxPair> distances, int take)
{
    var circuits = boxes
        .Select(x => new HashSet<Box>([x]))
        .ToList();
    for (int i = 0; i < take; i++)
    {
        circuits = MakeConnection(circuits, distances[i]);
    }
    return circuits;
}

List<HashSet<Box>> MakeConnection(List<HashSet<Box>> circuits, BoxPair pair)
{
    var indexOfFirst = circuits.FindIndex(x =>  x.Contains(pair.First));
    var indexOfSecond = circuits.FindIndex(x =>  x.Contains(pair.Second));

    if (indexOfFirst == indexOfSecond)
    {
        return circuits;
    }
    
    circuits[indexOfFirst] = [.. circuits[indexOfFirst], ..circuits[indexOfSecond]];
    circuits.RemoveAt(indexOfSecond);

    return circuits;
}

ImmutableArray<Box> GetJunctionBoxes() =>
[
    ..File.ReadAllLines("input.txt")
        .Select(x => x.Split(','))
        .Select(x => x.Select(int.Parse).ToImmutableArray())
        .Select(x => new Box(x[0], x[1], x[2]))
];

List<BoxPair> GetDistances(ImmutableArray<Box> immutableArray)
{
    var distances = new List<BoxPair>();
    foreach (var (box, i) in immutableArray.Zip(Enumerable.Range(0, immutableArray.Length)))
    {
        var otherBoxes = immutableArray[(i + 1)..];
        foreach (var otherBox in otherBoxes)
        {
            distances.Add(new(box, otherBox, GetDistance(box, otherBox)));
        }
    }
    
    return distances.OrderBy(x => x.Distance).ToList();

    double GetDistance(Box box1, Box box2) => 
        Math.Sqrt(
            DistPow2(box1.X, box2.X) + 
            DistPow2(box1.Y, box2.Y) + 
            DistPow2(box1.Z, box2.Z));

    double DistPow2(int x, int y) => 
        Math.Pow((x - y), 2);
}

sealed record Box(int X, int Y, int Z);

sealed record BoxPair(Box First, Box Second, double Distance);