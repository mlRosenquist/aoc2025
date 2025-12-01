using System.Collections.Immutable;

var dialPositions = 100;
var input = File.ReadAllLines("input.txt");
PartOne(input);
PartTwo(input);

void PartOne(string[] input)
{
    var instructions = GetInstructions(input);
    
    var dial = 50;
    var zeros = 0;
    foreach (var (dir, movement) in instructions)
    {
        dial = dir switch
        {
            'L' => RotateLeft(dial, movement).Dial,
            'R' => RotateRight(dial, movement).Dial,
            _ => throw new InvalidOperationException()
        };
        if (dial == 0)
        {
            zeros++;
        }
    }
    Console.WriteLine(zeros);
}

void PartTwo(string[] input)
{
    var instructions = GetInstructions(input);
    var dial = 50;
    var zeroTicks = 0;
    foreach (var (dir, movement) in instructions)
    {
        (dial, var times) = dir switch
        {
            'L' => RotateLeft(dial, movement),
            'R' => RotateRight(dial, movement),
            _ => throw new InvalidOperationException()
        };
        zeroTicks += times;
    }
    Console.WriteLine(zeroTicks);
}

(int Dial, int ZeroTicks) RotateRight(int dial, int amount)
{
    var sum = dial + amount;
    if (sum < dialPositions)
    {
        return (sum, 0);
    }
    
    var remainder = (sum % dialPositions);
    var times = sum / dialPositions;
    if(remainder == 0)
    {
        return (0, times);
    }
    
    return (remainder, times);
}

(int Dial, int ZeroTicks) RotateLeft(int dial, int movement)
{
    var sum = dial - movement;
    if (sum >= 0)
    {
        return (sum, sum == 0 ? 1 : 0);
    }
    
    if (sum > -dialPositions && dial != 0)
    {
        return (dialPositions - Math.Abs(sum), 1);
    }
    
    var remainderAmount = movement % dialPositions;
    var times = Math.Abs(movement / dialPositions) +  (remainderAmount >= dial && dial != 0 ? 1 : 0);
    var remainder = sum % dialPositions;

    if(remainder == 0)
    {
        return (0, times);
    }
    
    return (dialPositions - Math.Abs(remainder), times);
}

ImmutableArray<(char Dir, int Movement)> GetInstructions(string[] strings) =>
    [..strings.Select(x => (x[0], int.Parse(x[1..])))];


