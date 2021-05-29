public class Aosuke : AGhost
{
    public void Initialize(Maze maze, Pacman pacman, Akabei akabei)
    {
        transform.localPosition = maze.AosukeWaypointsData.Start;
        
        var ai = GetComponent<AosukeAI>();
        ai.Initialize(pacman, maze.TileUtility, akabei);
        
        var mover = GetComponent<Mover>();
        mover.Initialize(ai);
        
        base.Initialize(mover, maze.AosukeWaypointsData, maze.NestWaypoints);
    }
}
