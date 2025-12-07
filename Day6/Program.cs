using System.Collections.Immutable;
using System.Text.RegularExpressions;

PartOne();
PartTwo();

void PartOne() => 
    Console.WriteLine(GetEquations().Sum(x => x.Result()));

void PartTwo() => 
    Console.WriteLine(GetEquations2().Sum(x => x.Result()));

ImmutableArray<Equation> GetEquations()
{
    var input = File.ReadAllLines("input.txt");
    var firstLine = SplitOnWhiteSpace(input[0]);
    
    string[][] equations = new string[firstLine.Length][];
    for (int i = 0; i < firstLine.Length; i++)
    {
        equations[i] = new string[input.Length];
    }
    
    foreach (var (line, y) in input.Zip(Enumerable.Range(0, input.Length)))
    {
        var lineNumbers = SplitOnWhiteSpace(line);

        foreach (var (numberOrOperator, x) in lineNumbers.Zip(Enumerable.Range(0, lineNumbers.Length)))
        {
            equations[x][y] = numberOrOperator;
        }
    }

    return [
        ..equations.Select(x => new Equation(
            Numbers: [..x[..^1].Select(long.Parse)],
            Operator: x[^1][0]))
    ];
}

ImmutableArray<Equation> GetEquations2()
{
    var input = File.ReadAllLines("input.txt")
        .Select(x => x.ToCharArray())
        .ToImmutableArray();

    var numberRows = input[..^1];
    
    var equationCounter = 0;
    var numbers = new Dictionary<int, List<long>>();
    for (var x = 0; x < input[0].Length; x++)
    {
        if (numberRows.All(row => row[x] == ' '))
        {
            equationCounter++;
            continue;
        }

        var number = Enumerable.Range(0, numberRows.Length)
            .Aggregate(string.Empty, (s, i) => s + numberRows[i][x])
            .Trim();
        
        if (numbers.ContainsKey(equationCounter))
        {
            numbers[equationCounter].Add(long.Parse(number));
        }
        else
        {
            numbers[equationCounter] = [long.Parse(number)];
        }
    }
    
    var operations = SplitOnWhiteSpace(new string(input[^1]))
        .Select(x => x[0])
        .ToImmutableArray();

    return [..numbers.Zip(operations, (x, y) => new Equation([..x.Value], y))];
}

ImmutableArray<string> SplitOnWhiteSpace(string input) => 
[
    ..Regex.Replace(input, @"\s+", " ")
        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
];

record Equation(ImmutableArray<long> Numbers, char Operator)
{
    public long Result() =>
        Operator switch
        {
            '+' => Numbers.Sum(),
            '*' => Numbers.Aggregate(1L, (i, l) => i * l),
            _ => throw new ArgumentException()
        };
}
