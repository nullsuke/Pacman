using System.Collections.Generic;
using UnityEngine;

public class MazeUtility : MonoBehaviour
{
    private readonly List<Vector2> visited = new List<Vector2>();
    private Color[] pix;
    private int width;
    private int height;
    
    public List<Rectangle> GetRectangles(Texture2D sourceTex)
    {
        visited.Clear();
        width = sourceTex.width;
        height = sourceTex.height;
        pix = sourceTex.GetPixels(0, 0, width, height);

        var rects = new List<Rectangle>();

        for (int y = 0; y < height; y += Rectangle.Unit.y)
        {
            for (int x = 0; x < width; x += Rectangle.Unit.x)
            {
                var p = new Vector2(x, y);

                if (visited.Contains(p)) continue;

                int i = y * width + x;

                if (pix[i].a > 0)
                {
                    var rec = GetRectangle(p);
                    rects.Add(rec);
                }
            }
        }

        return rects;
    }

    private Rectangle GetRectangle(Vector2 p)
    {
        var rec = new Rectangle();
        var points = GetLinePoints(p);
        
        visited.AddRange(points);

        rec.BottomLeft = points[0];
        rec.Width = points.Count * Rectangle.Unit.x; ;

        int i;

        do
        {
            p.y += Rectangle.Unit.y;
            
            if (p.y >= height) break;

            points = GetLinePoints(p);

            if (rec.Width != points.Count * Rectangle.Unit.x)
            {
                break;
            }
            else
            {
                visited.AddRange(points);
            }

            i = (int)p.y * width + (int)p.x;
        } while (pix[i].a > 0);

        rec.Height = p.y - rec.BottomLeft.y;

        return rec;
    }

    private List<Vector2> GetLinePoints(Vector2 p)
    {
        var points = new List<Vector2>();
        int i = (int)p.y * width + (int)p.x;

        while (p.x < width && pix[i].a > 0)
        {
            points.Add(p);
            p.x += Rectangle.Unit.x;
            i = (int)p.y * width + (int)p.x;
        } 

        return points;
    }
}
