using System;
using UnityEngine;

public class Dot : MonoBehaviour, IEatable
{
    public event EventHandler OnEaten;
    private static readonly int score = 10;

    public int Score { get => score; }

    public void Eaten(Pacman pacman = null)
    {
        OnEaten(this, EventArgs.Empty);
        //Destroyだと処理が重いのでとりあえず非アクティブにしておく。
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
