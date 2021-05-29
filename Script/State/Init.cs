using System;
using System.Collections.Generic;
using UnityEngine;

public class Init : IState
{
    public event EventHandler OnTouch;
    private readonly Dictionary<State, IState> states;
    private readonly Queue<Vector2> waypoints;
    private readonly Mover mover;
    private readonly Animator animator;

    public Init(Mover mover, Dictionary<State, IState> states,
        Animator animator, Queue<Vector2> waypoints)
    {
        this.mover = mover;
        this.states = states;
        this.animator = animator;
        this.waypoints = waypoints;

        states.Add(State.Init, this);
    }

    //巡回の最初のWaypointへ移動し、到着したら巡回状態になる。
    public IState Excute()
    {
        IState next = this;

        mover.MoveToWaypointsThen(() =>
        {
            var scatter = states[State.Scatter] as Scatter;
            scatter.SetState();

            next = scatter;
        });

        return next;
    }

    public void SetState()
    {
        mover.SetWaypoints(waypoints);
    }

    public IState Frighten(float scareSpan)
    {
        var next = states[State.Scare] as Scare;
        next.SetState(this, scareSpan);

        return next;
    }

    public IState Calm()
    {
        return this;
    }

    public IState Eaten()
    {
        OnTouch(this, EventArgs.Empty);

        return this;
    }

    //準備中ワープは絶対しない。
    public void Warp(Vector2 pos, float warpSpan)
    {
        throw new System.NotImplementedException();
    }

    public void Animate()
    {
        animator.SetFloat("DirX", mover.Direction.x);
        animator.SetFloat("DirY", mover.Direction.y);
    }
}
