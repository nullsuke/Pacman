using System;
using UnityEngine;

public class PowerCokie : MonoBehaviour, IEatable
{
    public event EventHandler OnEaten;
    private static readonly int score = 50;

    public int Score { get => score; }

    public void Eaten(Pacman pacman = null)
    {
        OnEaten(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
