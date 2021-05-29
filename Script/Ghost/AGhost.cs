using System;
using System.Collections.Generic;
using UnityEngine;

public enum State { Wait, Init, Scatter, Chase, Scare, Return, Dead };

public abstract class AGhost : MonoBehaviour, IWarpable, IEatable
{
    //待機間隔。
	[SerializeField] protected float waitSpan;
    //巡回間隔。
	[SerializeField] protected float scatterSpan;
    //追跡間隔。
	[SerializeField] protected float chaseSpan;
	//[SerializeField] protected float scareTime;
    //パックマンに食べられたときの処理を登録。
	public event EventHandler OnEaten;
    //パックマンを食べたときの処理を登録。
	public event EventHandler OnEat;
    //ワープ時間。
    private readonly float warpSpan = 1f;
    private readonly int score = 200;
    private SpriteRenderer sprite;
    private Animator animator;
    private IState state;
    private Wait wait;
    private float pauseEndTime;

    public int Score { get => score; }

    //初期化。
    public void Initialize(Mover mover, GhostWaypointsData gwd, 
        List<Vector2> nestWaypoints)
    {
        var states = new Dictionary<State, IState>();
        pauseEndTime = 0;

        wait = new Wait(mover, states, animator, ToQueue(gwd.Wait), waitSpan);
        wait.OnTouch += (s, e) => OnEat(s, e);

        var init = new Init(mover, states, animator, ToQueue(gwd.Init));
        init.OnTouch += (s, e) => OnEat(s, e);

        var scatter = new Scatter(mover, states, animator, 
            ToQueue(gwd.Scatter), scatterSpan);
        scatter.OnTouch += (s, e) => OnEat(s, e);

        var chase = new Chase(mover, states, animator, sprite, chaseSpan);
        chase.OnTouch += (s, e) => OnEat(s, e);

        var scare = new Scare(mover, states, animator, sprite);
        scare.OnTouch += (s, e) => OnEaten(s, e);

        var ret = new Return(mover, states, animator, sprite,
            ToQueue(gwd.Scatter).Peek());
        ret.OnTouch += (s, e) => OnEat(s, e);

        var dead = new Dead(mover, states, animator,
            ToQueue(nestWaypoints));
        dead.OnTouch += (s, e) => { };

        enabled = false;
        animator.enabled = false;
    }

    //ワープ。
	public void Warp(Vector2 pos)
    {
        state.Warp(pos, warpSpan);
    }

    //パックマンに食べられる。
    public void Eaten(Pacman pacman)
    {
        state = state.Eaten();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    //恐慌。
    public void Scare(float span)
    {
        state = state.Frighten(span);
    }

    //恐慌終了。
    public void Calm()
    {
        state = state.Calm();
    }

    public void Run()
    {
        wait.SetState();
        state = wait;
        enabled = true;
        animator.enabled = true;
    }

    public void Pause(float span)
    {
        pauseEndTime = Time.fixedTime + span;
    }

    public void Stop()
    {
        enabled = false;
        animator.enabled = false;
    }

    private void Awake()
    {
		sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    //移動・アニメーション。
	private void FixedUpdate()
	{
        if (Time.fixedTime < pauseEndTime) return;

        state = state.Excute();
        state.Animate();
    }

    //List<Vector2>からQueue<Vector2>へ変換。
    private Queue<Vector2> ToQueue(List<Vector2> waypoints)
    {
        var wps = new Queue<Vector2>();

        waypoints.ForEach(w =>
        {
            wps.Enqueue(w);
        });

        return wps;
    }
}
