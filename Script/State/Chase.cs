using System;
using System.Collections.Generic;
using UnityEngine;

public class Chase : IState
{
    public event EventHandler OnTouch;
    private readonly Dictionary<State, IState> states;
    private readonly Mover mover;
    private readonly Animator animator;
    private readonly float span;
    private readonly SpriteRenderer sprite;
    //ワープ中がどうか。
    private bool warped;
    //残り時間。
    private float leftSpan;
    //追跡終了時間。
    private float limitTime;
    //ワープ終了時間。
    private float warpEndTime;

    public Chase(Mover mover, Dictionary<State, IState> states, 
        Animator animator, SpriteRenderer sprite, float span)
    {
        this.mover = mover;
        this.states = states;
        this.animator = animator;
        this.sprite = sprite;
        this.span = span;

        states.Add(State.Chase, this);
    }

    //一定時間追跡した後、帰還状態になる。
    public IState Excute()
    {
        if (!warped)
        {
            if (limitTime < Time.fixedTime)
            {
                leftSpan = 0;

                var next = states[State.Return] as Return;
                next.SetState();

                return next;
            }
            else
            {
                mover.MoveToWaypoints(State.Chase);
                return this;
            }
        }
        else //ワープ中。
        {
            if (warpEndTime < Time.fixedTime)
            {
                sprite.enabled = true;
                warped = false;
                limitTime = leftSpan + Time.fixedTime;
            }

            return this;
        }
    }

    public void SetState()
    {
        limitTime = Time.fixedTime + span;
    }

    public IState Frighten(float scareSpan)
    {
        //追跡時間を延長。
        limitTime += scareSpan;

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

        //追跡の残り時間を記録。
        leftSpan = Mathf.Max(limitTime - Time.fixedTime, 0);
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
