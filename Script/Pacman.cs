using UnityEngine;
using System;

public class Pacman : MonoBehaviour, IWarpable
{
    [SerializeField] private float speed;
    //死亡時のイベントを登録。
    public event EventHandler OnDead;
    private Rigidbody2D rigid2D;
    private Collider2D collider2d;
    private Animator animator;
    private Vector2 dest;
    private Vector2 nextDir;
    private float pauseLimit;

    public Vector2 Direction { get; private set; }

    public void Initialize(Vector2 startPosition)
    {
        transform.localPosition = startPosition;
        dest = transform.position;
        Direction = Vector2Int.down;
        nextDir = Direction;
        enabled = false;
        animator.enabled = false;
    }

    public void Warp(Vector2 pos)
    {
        transform.localPosition = pos;
        dest = (Vector2)transform.position + Direction;
    }

    public void Dead()
    {
        animator.SetBool("IsDead", true);
        enabled = false;
    }

    public void Run()
    {
        enabled = true;
        animator.enabled = true;
    }

    public void Pause(float span)
    {
        pauseLimit = Time.fixedTime + span;
    }

    public void Stop()
    {
        enabled = false;
        animator.enabled = false;
    }

    private void Destroy()
    {
        Destroy(gameObject);
        OnDead(this, EventArgs.Empty);
    }

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (Time.fixedTime < pauseLimit) return;

        Move();
        Animate();
    }

    private void Move()
    {
        var p = Vector2.MoveTowards(transform.position, dest, speed);
        rigid2D.MovePosition(p);

        if (Input.GetAxis("Horizontal") > 0) nextDir = Vector2.right;
        if (Input.GetAxis("Horizontal") < 0) nextDir = Vector2.left;
        if (Input.GetAxis("Vertical") > 0) nextDir = Vector2.up;
        if (Input.GetAxis("Vertical") < 0) nextDir = Vector2.down;
        
        if (Vector2.Distance(dest, transform.position) < float.Epsilon)
        {
            if (CanMove(nextDir))
            {
                dest = (Vector2)transform.position + nextDir;
                Direction = nextDir;
            }
            else
            {
                if (CanMove(Direction))
                {
                    dest = (Vector2)transform.position + Direction;
                }
            }
        }
    }

    private bool CanMove(Vector2 dir)
    {
        Vector2 p = transform.position;
        var hit = Physics2D.Linecast(p + dir, p);

        return hit.collider == collider2d;
    }

    private void Animate()
    {
        animator.SetFloat("DirX", Direction.x);
        animator.SetFloat("DirY", Direction.y);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var e = collider.GetComponent<IEatable>();
        e?.Eaten(this);
    }
}
