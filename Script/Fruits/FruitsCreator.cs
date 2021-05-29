using UnityEngine;

public class FruitsCreator : MonoBehaviour
{
    [SerializeField] private Fruits[] fruitsPrefabs = default;
    //フルーツ出現に必要なエサの個数。
    private readonly int[] limits = { 70, 170, int.MaxValue };
    private int limit;
    private int i;

    public void Initialize()
    {
        limit = limits[0];
        i = 0;
    }

    public bool HasEatenDotEnough(int cnt)
    {
        if (limit <= cnt)
        {
            i++;
            limit = limits[i];

            return true;
        }
        return false;
    }

    public Fruits Create(Maze maze, int mazeIndex)
    {
        var fruits = Instantiate(fruitsPrefabs[mazeIndex], maze.transform);
        //パックマンのスタート地点に出現させる。
        fruits.transform.localPosition = maze.PacmanStartPosition;

        return fruits;
    }
}
