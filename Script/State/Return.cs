using System;
using System.Collections.Generic;
using UnityEngine;

public class Return : IState
{
    public event EventHandler OnTouch;
    private readonly Dictionary<State, IState> states;
    private readonly Mover mover;
    private readonly Animator animator;
    private readonly Vector2 scatterPoint;
    private readonly SpriteRenderer sprite;
    private Scatter next;
    //ワープ中がどうか。
    private bool warped;
    //ワープ終了時間。
    private float warpEndTime;

    public Return(Mover mover, Dictionary<State, IState> states, 
        Animator animator, SpriteRenderer sprite, Vector2 scatterPoint)
    {
        this.mover = mover;
        this.states = states;
        this.animator = animator;
        this.sprite = sprite;
        this.scatterPoint = scatterPoint;

        states.Add(State.Return, this);
    }

    //巡回開始の座標まで移動。到着後は巡回状態になる。
    public IState Excute()
    {
        IState state = this;

        if (!warped)
        {
            if (next == null)
            {
                mover.MoveToWaypointsThen(scatterPoint,
                () =>
                {
                    next = states[State.Scatter] as Scatter;
                    next.SetState();
                });
            }
            else state = next;        
        }
        else //ワープ中。
        {
            if (warpEndTime < Time.fixedTime)
            {
                sprite.enabled = true;
                warped = false;
            }
        }

        return state;
    }

    public void SetState()
    {
        next = null;
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

    public void Warp(Vector2 pos, float warpSpan)
    {
        sprite.enabled = false;

        warpEndTime = Time.fixedTime + warpSpan;

        warped = true;
        mover.Warp(pos);
    }

    public void Animate()
    {
        animator.SetFloat("DirX", mover.Direction.x);
        animator.SetFloat("DirY", mover.Direction.y);
    }
}
