using System;
using System.Collections.Generic;
using UnityEngine;

public class Scare : IState
{
    public event EventHandler OnTouch;
    private readonly Dictionary<State, IState> states;
    private readonly Mover mover;
    private readonly Animator animator;
    private readonly float defaultSpeed;
    private readonly SpriteRenderer sprite;
    //元の状態。
    private IState preState;
    //ワープ中かどうか。
    private bool warped;
    //恐慌終了時間。
    private float limitTime;
    //ワープ終了時間。
    private float warpEndTime;

    public Scare(Mover mover, Dictionary<State, IState> states, 
        Animator animator, SpriteRenderer sprite)
    {
        this.mover = mover;
        this.states = states;
        this.animator = animator;
        this.sprite = sprite;
        defaultSpeed = mover.Speed;
        
        states.Add(State.Scare, this);
    }

    public IState Excute()
    {
        IState state = this;

        if (!warped)
        {
            if (preState is Wait || preState is Init ||
                    preState is Scatter)
            {
                //元の状態での行動を続ける。
                preState = preState.Excute();
            }
            else
            {
                //ランダムに移動。
                mover.MoveToWaypoints(State.Scare);
            }
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

    public void SetState(IState preState, float span)
    {
        this.preState = preState;
        limitTime = Time.fixedTime + span;
        
        mover.Speed = defaultSpeed * 0.8f;

        animator.SetBool("IsScare", true);
    }

    //恐慌中にパックパンがパワーエサを食べたら、恐慌時間は上書き。
    public IState Frighten(float span)
    {
        limitTime = Time.fixedTime + span;

        return this;
    }

    public IState Calm()
    {
        mover.Speed = defaultSpeed;
        sprite.enabled = true;
        warped = false;

        animator.SetBool("IsScare", false);
        animator.SetBool("IsCalmSoon", false);

        //恐慌状態が終わったら、元の状態になる。
        return preState;
    }

    public IState Eaten()
    {
        OnTouch(this, EventArgs.Empty);

        sprite.enabled = true;

        var next = states[State.Dead] as Dead;
        next.SetState();

        return next;
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
        //残り期間が3秒をきったら点滅。
        if (limitTime - Time.fixedTime < 3)
        {
            animator.SetBool("IsCalmSoon", true);
        }
        else
        {
            animator.SetBool("IsCalmSoon", false);
        }
    }
}
