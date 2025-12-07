using System.Collections.Immutable;
using System.Numerics;
using Map = System.Collections.Generic.Dictionary<System.Numerics.Complex, char>;

char Beam = '|';
char Splitter = '^';
char Empty = '.';

Complex East = new Complex(1, 0);
Complex West = new Complex(-1, 0);

PartOne();
PartTwo();

void PartOne()
{
    var input = File.ReadAllText("input.txt");
    var map = GetMap(input);
    var result = ReachableSplitters(map)
        .Count;
    Console.WriteLine(result);
}

void PartTwo()
{
    var input = File.ReadAllText("input.txt");
    var map = GetMap(input);
    var splitters = ReachableSplitters(map)
        .ToImmutableArray();
    var goalSplitters = splitters
        .Where(x => x.Value.NextLeftSplitter == null || x.Value.NextRightSplitter == null)
        .ToImmutableArray();
    
    var cache = new Dictionary<Complex, long>();
    var result = goalSplitters
        .Select(x => 
            (x.Value.NextLeftSplitter == null && x.Value.NextRightSplitter == null ? 2L : 1L) * CountPathsToSplitter(x.Key))
        .Sum();
    
    Console.WriteLine(result);
    
    long CountPathsToSplitter(Complex position)
    {
        if (cache.TryGetValue(position, out var value))
        {
            return value;
        }
        
        var parentSplitters = splitters.Where(x =>
                x.Value.NextLeftSplitter == position ||
                x.Value.NextRightSplitter == position)
            .ToImmutableArray();

        if (parentSplitters.Length == 0)
        {
            return 1;
        }
        
        var result = 
            parentSplitters.Sum(x => CountPathsToSplitter(x.Key));
        cache.Add(position, result);
        return result;
    }
}

ImmutableDictionary<Complex, Splitter> ReachableSplitters(Map map)
{
    var splitters = Splitters(map);
    var firstSplitter = splitters.OrderBy(x => x.Imaginary).First();
    var activeBeams = new Queue<Complex>([firstSplitter]);
    var result = new Dictionary<Complex, Splitter>();
    
    while (activeBeams.Count > 0)
    {
        var current = activeBeams.Dequeue();
        if (result.ContainsKey(current))
        {
            continue;
        }
        
        var nextLeftSplitter = splitters.FirstOrDefault(x =>
            x.Real == current.Real + West &&
            x.Imaginary > current.Imaginary);
        var nextRightSplitter = splitters.FirstOrDefault(x =>
            x.Real == current.Real + East &&
            x.Imaginary > current.Imaginary);
        
        Complex? leftSplitter = null;
        if (nextLeftSplitter != default)
        {
            leftSplitter = nextLeftSplitter;
            activeBeams.Enqueue(nextLeftSplitter);
        }
        
        Complex? rightSplitter = null;
        if (nextRightSplitter != default)
        {
            rightSplitter = nextRightSplitter;
            activeBeams.Enqueue(nextRightSplitter);
        }
        
        result.Add(current, new Splitter(leftSplitter, rightSplitter));
    }

    return result.ToImmutableDictionary();
}
    
Map GetMap(string input)
{
    var lines = input.Split("\r\n");
    var height = lines.Length;
    var width = lines[0].Length;

    return Enumerable.Range(0, height)
        .SelectMany(y => Enumerable.Range(0, width).Select(x => (new Complex(x, y), lines[y][x])))
        .ToDictionary(x => x.Item1, x => x.Item2);
}

ImmutableArray<Complex> Splitters(Map map) =>
[
    ..map.Where(x => x.Value == Splitter)
        .Select(x => x.Key)
        .OrderBy(x => x.Imaginary)
];

record Splitter(
    Complex? NextLeftSplitter,
    Complex? NextRightSplitter);