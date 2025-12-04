
using System.Collections.Immutable;
using System.Net.Mime;
using System.Numerics;
using Map = System.Collections.Generic.Dictionary<System.Numerics.Complex, char>;

char Free = '.';
char PaperRoll = '@';

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
void PartOne()
{
    var input = File.ReadAllText("input.txt");
    var map = GetMap(input);
    var paperRolls = PaperRollsWithLessThanFourAdjacent(map);
    
    SaveFiles(input, paperRolls);
    Console.WriteLine(paperRolls.Length);
}

void SaveFiles(string input, ImmutableArray<Complex> paperRolls)
{
    var lines = input.Split("\r\n");
    var height = lines.Length;
    var width = lines[0].Length;

    var updatedLines = Enumerable.Range(0, height)
        .Select(y => Enumerable.Range(0, width).Select(x => paperRolls.Contains(new(x, y)) ? 'x' : lines[y][x]))
        .Select(x => new string(x.ToArray()))
        .ToArray();
    File.WriteAllLines("originalOutput.txt", lines);
    File.WriteAllLines("output.txt", updatedLines);
}

ImmutableArray<Complex> PaperRollsWithLessThanFourAdjacent(Map map)
{
    var freePositions = map.Where(x => x.Value == PaperRoll)
        .Select(x => x.Key)
        .ToImmutableArray();
    
    var queue =  new Queue<Complex>(freePositions);
    var handledPaperRolls = new List<Complex>();
    while (queue.Count != 0)
    {
        var position = queue.Dequeue();

        if (AdjacentPaperRolls(map, position).Length < 4)
        {
            handledPaperRolls.Add(position);
            map[position] = Free;

            foreach (var direction in Directions)
            {
                var adjacent = position + direction;
                if (map.GetValueOrDefault(adjacent) == PaperRoll)
                {
                    queue.Enqueue(adjacent);
                    
                }
            }
        }
        // var adjacentValidPaperRolls = Directions
        //     .Select(direction => position + direction)
        //     .Where(potentialPosition =>
        //         IsPaperRoll(map, potentialPosition) && AdjacentPaperRolls(map, potentialPosition).Length < 4)
        //     .ToImmutableArray();
        //
        // handledPaperRolls.AddRange(adjacentValidPaperRolls);
    }
    return [..handledPaperRolls.ToImmutableHashSet()];
}

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
        .SelectMany(y => Enumerable.Range(0, width).Select(x => ((new Complex(x, y), lines[y][x]))))
        .ToDictionary(x =>  x.Item1, x => x.Item2);
}