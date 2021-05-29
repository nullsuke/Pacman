using UnityEngine;

public class Rectangle
{
    public static readonly Vector2Int Unit = new Vector2Int(2, 2);
    public Vector2 BottomLeft { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Width2 { get => Width * 2f; }
    public float Height2 { get => Height * 2f; }

    public Vector2 Center
    {
        get
        {
            var cx = BottomLeft.x + Width / 2f;
            var cy = BottomLeft.y + Height / 2f;

            return new Vector2(cx, cy);
        }
    }

    public Vector2 Center2
    {
        get
        {
            var cx = BottomLeft.x + Width;
            var cy = BottomLeft.y + Height;

            return new Vector2(cx, cy);
        }
    }

    public override string ToString()
    {
        return Center.ToString() + " w: " + Width.ToString() + " h: " + Height.ToString();
    }

    public string ToString2()
    {
        return Center2.ToString() + " w: " + Width2.ToString() + " h: " + Height2.ToString();
    }
}
