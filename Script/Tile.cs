using UnityEngine;

public class Tile
{
	public Tile Left, Right, Up, Down;
	public Vector2 Waypoint { get; private set; }
	public int X { get; private set; }
	public int Y { get; private set; }
	//行き止まりかどうか。
	public bool IsOccupied { get; set; } = true;
	//交差点かどうか。
	public bool IsIntersection { get; set; }
	//隣接しているTileの数。
	public int AdjacentCount { get; set; }
	
	public Tile(int x, int y)
	{
		X = x;
		Y = y;
		Waypoint = new Vector2(x, y);
		Left = Right = Up = Down = null;
	}

    public override string ToString()
    {
		string up = "";
		string right = "";
		string down = "";
		string left = "";

		if (Up != null) up = Up.Waypoint.ToString();
		if (Right != null) right = Right.Waypoint.ToString();
		if (Down != null) down = Down.Waypoint.ToString();
		if (Left != null) left = Left.Waypoint.ToString();

		return Waypoint.ToString() + ", Up: " + up + ", Right:" + right +
			", Down:" + down + ", Left" + left;
    }
}
