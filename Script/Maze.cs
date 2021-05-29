using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [SerializeField] public Vector2 PacmanStartPosition = default;
    [SerializeField] public GhostWaypointsData AkabeiWaypointsData = default;
    [SerializeField] public GhostWaypointsData PinkyWaypointsData = default;
    [SerializeField] public GhostWaypointsData AosukeWaypointsData = default;
    [SerializeField] public GhostWaypointsData GuzutaWaypointsData = default;
    [SerializeField] private Vector2Int size = default;
    [SerializeField] private Texture2D boxColliderMap;
    [SerializeField] private Texture2D dotMap;
    [SerializeField] private Texture2D powerCokieMap;
    [SerializeField] private Texture2D waypointsMap;
    [SerializeField] private Vector2[] nestWaypoints;
    //[SerializeField] private Texture2D ghostWaypointMap;
    
    private MazeUtility mazeUtility;
    private Animator animator;

    public List<Vector2> NestWaypoints { get => nestWaypoints.ToList<Vector2>(); }
    
    public TileUtility TileUtility { get; private set; }
    
    public List<Vector2> DotPositions
    {
        get
        {
            var dotRects = mazeUtility.GetRectangles(dotMap);
            var dots = new List<Vector2>();

            dotRects.ForEach(d =>
            {
                var c = d.Center2 / 8f;

                dots.Add(c); 
            });

            return dots;
        }
    }

    public List<Vector2> PowerCokiePositions
    {
        get
        {
            var powRects = mazeUtility.GetRectangles(powerCokieMap);
            var pows = new List<Vector2>();

            powRects.ForEach(d =>
            {
                var c = d.Center2 / 8f;

                pows.Add(c);
            });

            return pows;
        }
    }

    public void Blinking()
    {
        animator.enabled = true;
    }

    private void Start()
    {
        var boxes = mazeUtility.GetRectangles(boxColliderMap);

        boxes.ForEach(b =>
        {
            var c = b.Center2 / 8f;
            var w = b.Width2 / 8f;
            var h = b.Height2 / 8f;
            var size = new Vector2(w, h);

            AddBoxCollider2D(c, size);
        });

        //var gwps = mazeUtility.GetRectangles(ghostWaypointMap);

        //foreach (var p in gwps)
        //{
        //    var c = p.Center2 / 8;
        //    Debug.Log(c.ToString());
        //}
    }

    private void Awake()
    {
        mazeUtility = GetComponent<MazeUtility>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
        TileUtility = new TileUtility(CreateTiles(waypointsMap));
    }

    private void AddBoxCollider2D(Vector2 offset, Vector2 size)
    {
        var box = gameObject.AddComponent<BoxCollider2D>();
        box.offset = offset;
        box.size = size;
    }

    private List<Tile> CreateTiles(Texture2D waypointsMap)
    {
        var tiles = new List<Tile>();

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x + 1; x++)
            {
                var tile = new Tile(x, y);
                tiles.Add(tile);
            }
        }

        //WaypointsMapからRectangleを取得。
        var wayrects = mazeUtility.GetRectangles(waypointsMap);

        //Rectangleの中心座標がWaypoint。
        tiles.ForEach(t =>
        {
            if (wayrects.Exists(w => t.Waypoint == w.Center2 / 8f))
            {
                t.IsOccupied = false;
            }
        });

        SetWarpPoint(tiles);

        //Tileの上下左右にTileがあるか精査。
        foreach (var t in tiles.Where(t => !t.IsOccupied))
        {
            var up = tiles.Find(t2 => t2.Waypoint == t.Waypoint + Vector2.up);

            if (up != null && !up.IsOccupied)
            {
                t.Up = up;
                t.AdjacentCount++;
            }

            var right = tiles.Find(t2 => t2.Waypoint == t.Waypoint + Vector2.right);

            if (right != null && !right.IsOccupied)
            {
                t.Right = right;
                t.AdjacentCount++;
            }

            var down = tiles.Find(t2 => t2.Waypoint == t.Waypoint + Vector2.down);

            if (down != null && !down.IsOccupied)
            {
                t.Down = down;
                t.AdjacentCount++;
            }

            var left = tiles.Find(t2 => t2.Waypoint == t.Waypoint + Vector2.left);

            if (left != null && !left.IsOccupied)
            {
                t.Left = left;
                t.AdjacentCount++;
            }
        }

        //3つ以上Tileが隣接していたら、交差点。
        tiles.ForEach(t =>
        {
            if (t.AdjacentCount > 2) t.IsIntersection = true;
        });

        return tiles;
    }

    //ワープ地点のTileを追加。
    private void SetWarpPoint(List<Tile> tiles)
    {
        foreach(var w in GetComponentsInChildren<WarpPoint>())
        {
            var p = w.transform.localPosition;
            var tile = new Tile((int)p.x, (int)p.y);
            tile.IsOccupied = false;
            tiles.Add(tile);
        }
    }
}
