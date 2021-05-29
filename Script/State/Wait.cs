using System;
using System.Collections.Generic;
using UnityEngine;

public class Wait : IState
{
    public event EventHandler OnTouch;
    private readonly Dictionary<State, IState> states;
    private readonly Queue<Vector2> waypoints;
    private readonly Mover mover;
    private readonly Animator animator;
    //待機期間。
    private readonly float span;
    //待機終了時間。
    private float limitTime;

    public Wait(Mover mover, Dictionary<State, IState> states, 
        Animator animator, Queue<Vector2> waypoints, float span)
    {
        this.mover = mover;
        this.states = states;
        this.animator = animator;
        this.waypoints = waypoints;
        this.span = span;
        mover.SetWaypoints(waypoints);

        states.Add(State.Wait, this);
    
    }

    //一定時間巣で待機した後、準備状態になる。
    public IState Excute()
    {
        if (limitTime < Time.fixedTime)
        {
            var next = states[State.Init] as Init;
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

    //待機中ワープは絶対しない。
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
