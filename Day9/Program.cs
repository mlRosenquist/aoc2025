// See https://aka.ms/new-console-template for more information

using System.Collections.Immutable;
using System.Numerics;

PartOne();

void PartOne()
{
    var redTiles = GetRedTiles();
    var pairs = GetPairs(redTiles);
    var largest = pairs.Last();
    
    Console.WriteLine(largest.Rectangle);
}

void PartTwo()
{
    var redTiles = GetRedTiles();
    var lines = GetLines(redTiles)
        .ToImmutableArray();
    
    
    var pairs = GetPairs(redTiles);


    var largest = pairs.Last();
    
    Console.WriteLine(largest.Rectangle);
}

ImmutableArray<TilePair> GetValidPairs(ImmutableArray<TilePair> pairs, ImmutableArray<TilePair> lines)
{

    bool IsValid(TilePair pair, ImmutableArray<TilePair> lines)
    {
        var firstX = pair.First.X;
        var firstY = pair.First.Y;
        var secondX = pair.Second.X;
        var secondY = pair.Second.Y;
    }
}

IEnumerable<TilePair> GetLines(ImmutableArray<Tile> redTiles)
{
    for (int i = 0; i < redTiles.Length; i++)
    {
        if (i == redTiles.Length - 1)
        {
            yield return new TilePair(redTiles[i], redTiles[0], 0, 0);
        }
        
        yield return new TilePair(redTiles[i], redTiles[i + 1], 0, 0);
    }
}

List<TilePair> GetPairs(ImmutableArray<Tile> redTiles)
{
    var distances = new List<TilePair>();
    foreach (var (tile, i) in redTiles.Zip(Enumerable.Range(0, redTiles.Length)))
    {
        var otherTiles = redTiles[(i + 1)..];
        foreach (var otherTile in otherTiles)
        {
            distances.Add(new(tile, otherTile, GetDistance(tile, otherTile), GetRectangle(tile, otherTile)));
        }
    }
    
    return distances.OrderBy(x => x.Rectangle).ToList();

    int GetDistance(Tile first, Tile second) => 
        Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y);
    
    long GetRectangle(Tile first, Tile second) => 
        (long)(Math.Abs(first.X - second.X) + 1) * (long)(Math.Abs(first.Y - second.Y) + 1);
}



ImmutableArray<Tile> GetRedTiles() =>
[
    ..File.ReadAllLines("input.txt")
        .Select(x => x.Split(','))
        .Select(x => new Tile(int.Parse(x[0]), int.Parse(x[1])))
];

sealed record Tile(int X, int Y);

record TilePair(Tile First, Tile Second, int Distance, long Rectangle);

