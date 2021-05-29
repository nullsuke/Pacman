using UnityEngine;

public class GuzutaAI : AAI
{
    protected override Tile GetTargetTile()
    {
        var limit = 64;
        var dis = GetSqrDistance(pacman.transform.localPosition,
            transform.localPosition);
        Vector2 targetPos = Vector2.zero;

        if (dis >= limit) targetPos = pacman.transform.localPosition;
     
        return tileUtility.GetTile(targetPos);
    }
}
