using System.Collections.Immutable;

PartOne();
PartTwo();

void PartOne() =>
    Console.WriteLine(GetPairs(GetRedTiles())
        .Max(x => x.Rectangle));

void PartTwo()
{
    var redTiles = GetRedTiles();
    Console.WriteLine(GetValidPairs([..GetPairs(redTiles)], [..GetLines(redTiles)])
        .Max(x => x.Rectangle));
}

ImmutableArray<TilePair> GetValidPairs(ImmutableArray<TilePair> pairs, ImmutableArray<Line> lines)
{
    return [.. pairs.Where(IsValid)];

    bool IsValid(TilePair pair)
    {
        var (minX, maxX) = pair.First.X > pair.Second.X 
            ? (pair.Second.X, pair.First.X) 
            : (pair.First.X,  pair.Second.X);
        var (minY, maxY) = pair.First.Y > pair.Second.Y
            ? (pair.Second.Y, pair.First.Y) 
            : (pair.First.Y,  pair.Second.Y);

        var northLine = new Line(new(minX, minY), new(maxX, minY), Side.North);
        var eastLine = new Line(new(maxX, minY), new(maxX, maxY), Side.East);
        var southLine = new Line(new(minX, maxY), new(maxX, maxY), Side.South);
        var westLine = new Line(new(minX, minY), new(minX, maxY), Side.West);

        return !lines.Any(x => northLine.CrossLine(x) || eastLine.CrossLine(x) || southLine.CrossLine(x) || westLine.CrossLine(x));
    }
}

IEnumerable<Line> GetLines(ImmutableArray<Tile> redTiles)
{
    var tiles = new LinkedList<Tile>(redTiles);

    foreach (var tile in tiles)
    {
        var current = tiles.Find(tile)!;
        var previous = current.Previous ?? tiles.Last!;
        var next = current.Next ?? tiles.First!;

        yield return Line.Create(
            previous: new(previous.Value, current.Value), 
            current: new(current.Value, next.Value),
            next: new(next.Value, next.Next?.Value ?? tiles.First!.Value));
    }
}

IEnumerable<TilePair> GetPairs(ImmutableArray<Tile> redTiles)
{
    foreach (var (tile, i) in redTiles.Zip(Enumerable.Range(0, redTiles.Length)))
    {
        var otherTiles = redTiles[(i + 1)..];
        foreach (var otherTile in otherTiles)
        {
            yield return new(tile, otherTile);
        }
    }
}



ImmutableArray<Tile> GetRedTiles() =>
[
    ..File.ReadAllLines("input.txt")
        .Select(x => x.Split(','))
        .Select(x => new Tile(int.Parse(x[0]), int.Parse(x[1])))
];

sealed record Tile(int X, int Y);

record TilePair(Tile First, Tile Second)
{
    public bool IsHeadingNorth =
        First.Y > Second.Y;

    public bool IsHeadingEast =
        First.X < Second.X;

    public bool IsHeadingSouth = 
        First.Y < Second.Y;

    public bool IsHeadingWest = 
        First.X > Second.X;

    public long Rectangle=>
        (long)(Math.Abs(First.X - Second.X) + 1) * (long)(Math.Abs(First.Y - Second.Y) + 1);
};

enum Side
{
    North,
    East,
    South,
    West
}

record Line(Tile First, Tile Second, Side Side)
{
    public int MinX = Math.Min(First.X, Second.X);
    public int MaxX = Math.Max(First.X, Second.X);
    public int MinY = Math.Min(First.Y, Second.Y);
    public int MaxY = Math.Max(First.Y, Second.Y);

    public static Line Create(TilePair previous, TilePair current, TilePair next)
    {
        if (previous.IsHeadingNorth && current.IsHeadingEast && next.IsHeadingSouth)
        {
            return new(current.First, current.Second, Side.North);
        }
        if (previous.IsHeadingSouth && current.IsHeadingEast && next.IsHeadingNorth)
        {
            return new(current.First with { X = current.First.X + 1 }, current.Second with { X = current.Second.X - 1 }, Side.North);
        }
        if (previous.IsHeadingSouth && current.IsHeadingEast && next.IsHeadingSouth)
        {
            return new(current.First with { X = current.First.X + 1}, current.Second, Side.North);
        }
        if (previous.IsHeadingNorth && current.IsHeadingEast && next.IsHeadingNorth)
        {
            return new(current.First, current.Second with { X = current.Second.X - 1 }, Side.North);
        }

        if (previous.IsHeadingEast && current.IsHeadingSouth && next.IsHeadingWest)
        {
            return new(current.First, current.Second, Side.East);
        }
        if (previous.IsHeadingWest && current.IsHeadingSouth && next.IsHeadingEast)
        {
            return new(current.First with { Y = current.First.Y + 1 }, current.Second with { Y = current.Second.Y - 1 }, Side.East);
        }
        if (previous.IsHeadingWest && current.IsHeadingSouth && next.IsHeadingWest)
        {
            return new(current.First with { Y = current.First.Y + 1}, current.Second, Side.East);
        }
        if (previous.IsHeadingEast && current.IsHeadingSouth && next.IsHeadingEast)
        {
            return new(current.First, current.Second with { Y = current.Second.Y - 1 }, Side.East);
        }

        if (previous.IsHeadingNorth && current.IsHeadingWest && next.IsHeadingSouth)
        {
            return new(current.First with { X = current.First.X - 1 }, current.Second with { X = current.Second.X + 1 }, Side.South);
        }
        if (previous.IsHeadingSouth && current.IsHeadingWest && next.IsHeadingNorth)
        {
            return new(current.First, current.Second, Side.South);
        }
        if (previous.IsHeadingSouth && current.IsHeadingWest && next.IsHeadingSouth)
        {
            return new(current.First, current.Second with { X = current.Second.X + 1 }, Side.South);
        }
        if (previous.IsHeadingNorth && current.IsHeadingWest && next.IsHeadingNorth)
        {
            return new(current.First with { X = current.First.X - 1 }, current.Second, Side.South);
        }

        if (previous.IsHeadingEast && current.IsHeadingNorth && next.IsHeadingWest)
        {
            return new(current.First with { Y = current.First.Y - 1 }, current.Second with { Y = current.Second.Y + 1 }, Side.West);
        }
        if (previous.IsHeadingWest && current.IsHeadingNorth && next.IsHeadingEast)
        {
            return new(current.First, current.Second, Side.West);
        }
        if (previous.IsHeadingWest && current.IsHeadingNorth && next.IsHeadingWest)
        {
            return new(current.First, current.Second with { Y = current.Second.Y + 1 }, Side.West);
        }
        if (previous.IsHeadingEast && current.IsHeadingNorth && next.IsHeadingEast)
        {
            return new(current.First with { Y = current.First.Y - 1 }, current.Second, Side.West);
        }

        throw new ArgumentException();
    }

    public bool CrossLine(Line line)
    {
        if (Side == Side.North || Side == Side.South)
        {
            if (line.Side == Side.West)
            {
                return MinX < line.MinX && line.MinX <= MaxX && (line.MinY <= MinY && MinY <= line.MaxY);
            }
            if (line.Side == Side.East)
            {
                return MinX <= line.MinX && line.MinX < MaxX && (line.MinY <= MinY && MinY <= line.MaxY);
            }
        }

        if (Side == Side.West || Side == Side.East)
        {
            if (line.Side == Side.South)
            {
                return MinY <= line.MinY && line.MinY < MaxY && (line.MinX <= MinX && MinX <= line.MaxX);
            }
            if (line.Side == Side.North)
            {
                return MinY < line.MinY && line.MinY <= MaxY && (line.MinX <= MinX && MinX <= line.MaxX);
            }
        }

        return false;
    }
};