public class AkabeiAI : AAI
{
    protected override Tile GetTargetTile()
    {
        return tileUtility.GetTile(pacman.transform.localPosition);
    }
}
