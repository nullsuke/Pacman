using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileUtility
{
    private readonly List<Tile> tiles;
    private readonly int maxX;
    private readonly int maxY;
    private readonly int minX;
    private readonly int minY;

    public TileUtility(List<Tile> tiles)
    {
        this.tiles = tiles;
        maxX = tiles.Max(t => t.X);
        maxY = tiles.Max(t => t.Y);
        minX = tiles.Min(t => t.X);
        minY = tiles.Min(t => t.Y);
    }

    public Tile GetTile(Vector2 pos)
    {
        var p = RoundAwayFromZero(pos);

        if (p.x < minX) p.x = minX;
        if (p.y < minY) p.y = minY;
        if (p.x > maxX) p.x = maxX;
        if (p.y > maxY) p.y = maxY;

        var tile = tiles.Find(t => t.Waypoint == p);

        if (tile == null) throw new Exception(p.ToString() + " tile not found");

        return tile;
    }

    private Vector2Int RoundAwayFromZero(Vector2 v)
    {
        int x = (int)(v.x + 0.5f);
        int y = (int)(v.y + 0.5f);

        return new Vector2Int(x, y);
    }
}
