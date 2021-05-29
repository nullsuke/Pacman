using System;

public interface IEatable
{
    event EventHandler OnEaten;    
    int Score { get; }
    void Eaten(Pacman pacman = null);
    void Destroy();
}
