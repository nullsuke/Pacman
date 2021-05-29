using System;
using System.Collections.Generic;
using UnityEngine;

public class Dead : IState
{
    public event EventHandler OnTouch;
    private readonly Dictionary<State, IState> states;
    private readonly Mover mover;
    private readonly Animator animator;
    //巣の付近の座標。
    private readonly Queue<Vector2> nestWaypoints;
    private readonly SoundManager soundManager;
    private readonly float defaultSpeed;
    //巣の近くにいるかどうか。
    private bool isArriveNearNest;
    
    public Dead(Mover mover, Dictionary<State, IState> states,
        Animator animator, Queue<Vector2> nestWaypoints)
    {
        this.mover = mover;
        this.states = states;
        this.animator = animator;
        this.nestWaypoints = nestWaypoints;
        defaultSpeed = mover.Speed;

        states.Add(State.Dead, this);

        soundManager = SoundManager.Instance;
    }

    public IState Excute()
    {
        IState next = this;

        //巣の近くに来るまでの移動。
        if (!isArriveNearNest)
        {
            mover.MoveToWaypointsThen(nestWaypoints.Peek(), 
                () => isArriveNearNest = true);
        }
        else //巣の付近での移動。巣に戻ったら、待機状態になる。
        {
            mover.MoveToWaypointsThen(() =>
            {
                mover.Speed = defaultSpeed;
                isArriveNearNest = false;

                var wait = states[State.Wait] as Wait;
                wait.SetState();

                next = wait;

                animator.SetBool("IsDead", false);
                soundManager.StopSE();
            });
        }

        return next;
    }

    public void SetState()
    {
        mover.SetWaypoints(nestWaypoints);
        mover.Speed = defaultSpeed *2f;

        animator.SetBool("IsScare", false);
        animator.SetBool("IsCalmSoon", false);
        animator.SetBool("IsDead", true);

        soundManager.PlaySE("GhostDead", 0.2f);
    }

    public IState Frighten(float scareSpan)
    {
        return this;
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
        mover.Warp(pos);
    }

    public void Animate()
    {
        animator.SetFloat("DirX", mover.Direction.x);
        animator.SetFloat("DirY", mover.Direction.y);
    }
}
