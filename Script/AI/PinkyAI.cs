using UnityEngine;

public class PinkyAI : AAI
{
    protected override Tile GetTargetTile()
    {
        var targetPos = (Vector2)pacman.transform.localPosition +
            pacman.Direction * 4;
        var tile = tileUtility.GetTile(targetPos);

        return tile;
    }
}
