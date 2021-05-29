using System;
using System.Collections.Generic;
using UnityEngine;

public class Scatter : IState
{
    public event EventHandler OnTouch;
    private readonly Dictionary<State, IState> states;
    //巡回する座標。
    private readonly Queue<Vector2> waypoints;
    private readonly Mover mover;
    private readonly Animator animator;
    //巡回期間。
    private readonly float span;
    //巡回終了時間。
    private float limitTime;

    public Scatter(Mover mover, Dictionary<State, IState> states,
        Animator animator, Queue<Vector2> waypoints, float span)
    {
        this.mover = mover;
        this.states = states;
        this.animator = animator;
        this.waypoints = waypoints;
        this.span = span;

        states.Add(State.Scatter, this);
    }

    //一定時間巡回した後、追跡状態になる。
    public IState Excute()
    {
        if (limitTime < Time.fixedTime)
        {
            var next = states[State.Chase] as Chase;
            next.SetState();

            return next;
        }

        mover.MoveToWaypoints(true);

        return this;
    }

    public void SetState()
    {
        limitTime = Time.fixedTime + span;
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

    //巡回中ワープは絶対しない。
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
