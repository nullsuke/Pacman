using UnityEngine;

public class AosukeAI : AAI
{
    private Akabei akabei;

    public void Initialize(Pacman pacman, TileUtility tileUtility,
        Akabei akabei)
    {
        this.akabei = akabei;
        base.Initialize(pacman, tileUtility);
    }

    protected override Tile GetTargetTile()
    {
        var forwardPos = (Vector2)pacman.transform.localPosition +
            pacman.Direction * 2;
        var ambushVector = forwardPos -
            (Vector2)akabei.transform.localPosition;
        var targetPos = forwardPos + ambushVector;
        var tile = tileUtility.GetTile(targetPos);

        return tile;
    }
}
