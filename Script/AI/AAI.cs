using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AAI : MonoBehaviour
{
    protected Pacman pacman;
    protected TileUtility tileUtility;
    
    public void Initialize(Pacman pacman, TileUtility tileUtility)
    {
        this.pacman = pacman;
        this.tileUtility = tileUtility;
    }

    public Vector2 GetWaypoint(Vector2 dir, State? state = null, Vector2? tp = null)
    {
        var currentPos = (Vector2)transform.localPosition;
        var currentTile = tileUtility.GetTile(currentPos);
        var nextTile = GetNextTile(dir, currentPos);
        var waypoint = currentPos;

        //移動できないか、交差点の場合。
        if (nextTile.IsOccupied || currentTile.IsIntersection)
        {
            //移動できなくて、かつ交差点でもない場合。
            if (nextTile.IsOccupied && !currentTile.IsIntersection)
            {
                if (dir.x != 0)
                {
                    if (currentTile.Up != null) waypoint = currentTile.Up.Waypoint;
                    if (currentTile.Down != null) waypoint = currentTile.Down.Waypoint;
                    
                    if (currentTile.Up == null && currentTile.Down == null)
                    {
                        if (dir.x > 0) waypoint = currentTile.Left.Waypoint;
                        else waypoint = currentTile.Right.Waypoint;
                    }
                }

                if (dir.y != 0)
                {
                    if (currentTile.Right != null) waypoint = currentTile.Right.Waypoint;
                    if (currentTile.Left != null) waypoint = currentTile.Left.Waypoint;
                    
                    if (currentTile.Right == null && currentTile.Left == null)
                    {
                        if (dir.y > 0) waypoint = currentTile.Down.Waypoint;
                        else waypoint = currentTile.Up.Waypoint;
                    }
                }
            }

            //交差点の場合。
            if (currentTile.IsIntersection)
            {
                switch (state)
                {
                    //追跡状態のとき対象に向けて移動。
                    case State.Chase:
                        tp = GetTargetTile().Waypoint;
                        waypoint = GetWaypointAtIntersection(dir, currentTile, tp.Value);
                        break;
                    //恐慌状態のときランダムに移動。
                    case State.Scare:
                        waypoint = GetWaypointAtIntersection(dir, currentTile);
                        break;
                    //その他。
                    case null:
                        waypoint = GetWaypointAtIntersection(dir, currentTile, tp.Value);
                        break;
                    default:
                        break;
                }
            }

            return waypoint;
        }
        else //移動できて、かつ交差点でない場合。
        {
            return nextTile.Waypoint;
        }
    }

    //追跡対象のtileを取得。実装は各ゴーストでする。
    protected abstract Tile GetTargetTile();

    protected float GetSqrDistance(Vector2 a, Vector2 b)
    {
        return Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2);
    }

    private Tile GetNextTile(Vector2 dir, Vector2 pos)
    {
        Tile nextTile = null;

        if (dir.x > 0)
        {
            nextTile = tileUtility.GetTile(pos + Vector2.right);
        }
        if (dir.x < 0)
        {
            nextTile = tileUtility.GetTile(pos + Vector2.left);
        }
        if (dir.y > 0)
        {
            nextTile = tileUtility.GetTile(pos + Vector2.up);
        }
        if (dir.y < 0)
        {
            nextTile = tileUtility.GetTile(pos + Vector2.down);
        }

        return nextTile;
    }

    //tpへの直線距離が近いwaypointを取得。
    private Vector2 GetWaypointAtIntersection(Vector2 dir, Tile currentTile, Vector2 tp)
    {
        var waypoint = currentTile.Waypoint;

        float up, right, down, left;
        up = right = down = left = Mathf.Infinity;

        if (currentTile.Up != null && !(dir.y < 0))
        {
            up = GetSqrDistance(tp, currentTile.Up.Waypoint);
        }

        if (currentTile.Right != null && !(dir.x < 0))
        {
            right = GetSqrDistance(tp, currentTile.Right.Waypoint);
        }

        if (currentTile.Down != null && !(dir.y > 0))
        {
            down = GetSqrDistance(tp, currentTile.Down.Waypoint);
        }

        if (currentTile.Left != null && !(dir.x > 0))
        {
            left = GetSqrDistance(tp, currentTile.Left.Waypoint);
        }

        var min = Mathf.Min(up, right, down, left);

        if (min == up) waypoint = currentTile.Up.Waypoint;
        else if (min == right) waypoint = currentTile.Right.Waypoint;
        else if (min == down) waypoint = currentTile.Down.Waypoint;
        else if (min == left) waypoint = currentTile.Left.Waypoint;

        if (waypoint == currentTile.Waypoint) throw new Exception("no way");

        return waypoint;
    }

    //現在のtileからランダムにwaypointを取得。
    private Vector2 GetWaypointAtIntersection(Vector2 dir, Tile currentTile)
    {
        var available = new List<Tile>();

        if (currentTile.Up != null && !(dir.y < 0))
        {
            available.Add(currentTile.Up);
        }

        if (currentTile.Right != null && !(dir.x < 0))
        {
            available.Add(currentTile.Right);
        }

        if (currentTile.Down != null && !(dir.y > 0))
        {
            available.Add(currentTile.Down);
        }

        if (currentTile.Left != null && !(dir.x > 0))
        {
            available.Add(currentTile.Left);
        }

        var m = available.Count;
        var r = UnityEngine.Random.Range(0, m);

        return available[r].Waypoint;
    }
}
