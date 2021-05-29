public class Guzuta : AGhost
{
    public void Initialize(Maze maze, Pacman pacman)
    {
        transform.localPosition = maze.GuzutaWaypointsData.Start;

        var ai = GetComponent<GuzutaAI>();
        ai.Initialize(pacman, maze.TileUtility);
        
        var mover = GetComponent<Mover>();
        mover.Initialize(ai);
        
        base.Initialize(mover, maze.GuzutaWaypointsData, maze.NestWaypoints);
    }
}
