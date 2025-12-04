
using System.Collections.Immutable;
using System.Numerics;
using Map = System.Collections.Generic.Dictionary<System.Numerics.Complex, char>;

char Free = '.';
char PaperRoll = '@';
const int MaxNeighbors = 3;

Complex North = new Complex(0, -1);
Complex East = new Complex(1, 0);
Complex South = new Complex(0, 1);
Complex West = new Complex(-1, 0);
Complex NorthEast = North + East;
Complex SouthEast = South + East;
Complex NorthWest = North + West;
Complex SouthWest = South + West;
ImmutableArray<Complex> Directions = [North, East, South, West, NorthEast, SouthEast, NorthWest, SouthWest];

PartOne();
PartTwo();
void PartOne()
{
    var input = File.ReadAllText("input.txt");
    var map = GetMap(input);
    var paperRolls = PaperRollsAndAmountAdjacent(map)
        .Where(x => x.Value < MaxNeighbors)
        .ToImmutableArray();
    
    Console.WriteLine(paperRolls.Length);
}

void PartTwo()
{
    var input = File.ReadAllText("input.txt");
    var map = GetMap(input);
    var paperRolls = PaperRollsAndAmountAdjacent(map);

    var result = new List<Complex>();

    while (true)
    {
        var removables =  paperRolls.Where(x => x.Value <= MaxNeighbors)
            .Select(x => (x.Key, x.Value))
            .ToImmutableArray();
        if (removables.Length <= 0)
        {
            break;
        }

        foreach (var paperRoll in removables)
        {
            result.Add(paperRoll.Key);
            paperRolls.Remove(paperRoll.Key);
            foreach (var direction in Directions)
            {
                var adjacent = paperRoll.Key + direction;
                if (paperRolls.TryGetValue(adjacent, out var value))
                {
                    paperRolls[adjacent] = value - 1;
                }
            }       
        }
    }

    Console.WriteLine(result.Count);
}

Dictionary<Complex, int> PaperRollsAndAmountAdjacent(Map map) =>
    map.Where(x => x.Value == PaperRoll)
        .ToDictionary(x => x.Key, x => AdjacentPaperRolls(map, x.Key).Length);

ImmutableArray<Complex> AdjacentPaperRolls(Map map, Complex position) =>
[
    ..Directions
        .Select(direction => position + direction)
        .Where(potentialPosition => IsPaperRoll(map, potentialPosition))
];

bool IsPaperRoll(Map map, Complex position) =>
    map.GetValueOrDefault(position) == PaperRoll;

Map GetMap(string input)
{
    var lines = input.Split("\r\n");
    var height = lines.Length;
    var width = lines[0].Length;

    return Enumerable.Range(0, height)
        .SelectMany(y => Enumerable.Range(0, width).Select(x => (new Complex(x, y), lines[y][x])))
        .ToDictionary(x =>  x.Item1, x => x.Item2);
}