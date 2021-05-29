public class Akabei : AGhost
{
    public void Initialize(Maze maze, Pacman pacman)
    {
        transform.localPosition = maze.AkabeiWaypointsData.Start;
        
        var ai = GetComponent<AkabeiAI>();
        ai.Initialize(pacman, maze.TileUtility);
        
        var mover = GetComponent<Mover>();
        mover.Initialize(ai);

        base.Initialize(mover, maze.AkabeiWaypointsData, maze.NestWaypoints);
    }
}
