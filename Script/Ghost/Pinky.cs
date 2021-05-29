public class Pinky : AGhost
{
    public void Initialize(Maze maze, Pacman pacman)
    {
        transform.localPosition = maze.PinkyWaypointsData.Start;

        var ai = GetComponent<PinkyAI>();
        ai.Initialize(pacman, maze.TileUtility);

        var mover = GetComponent<Mover>();
        mover.Initialize(ai);

        base.Initialize(mover, maze.PinkyWaypointsData, maze.NestWaypoints);
    }
}
