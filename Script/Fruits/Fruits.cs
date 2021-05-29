using System;
using System.Collections;
using UnityEngine;

public class Fruits : MonoBehaviour, IEatable
{
    public event EventHandler OnEaten;
    protected int score;
    //表示時間。
    private readonly static int limit = 10;

    public int Score => score;

    public void Eaten(Pacman pacman = null)
    {
        OnEaten(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(CroutineAppear());
    }

    //limit秒後削除。
    private IEnumerator CroutineAppear()
    {
        yield return new WaitForSeconds(limit);
        Destroy(gameObject);
    }
}
