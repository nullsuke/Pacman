using System;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
	[SerializeField] float defaultSpeed = default;
	private Queue<Vector2> waypoints;
	private AAI ai;
	private Rigidbody2D rigid2D;
	private Vector2 waypoint;
	
	public Vector2 Direction { get; private set; }
	public float Speed { get; set; }

	public void Initialize(AAI ai)
	{
		this.ai = ai;
		Speed = defaultSpeed;
	}

	private void Awake()
	{
		rigid2D = GetComponent<Rigidbody2D>();
	}

	public void SetWaypoints(Queue<Vector2> waypoints)
	{
		this.waypoints = new Queue<Vector2>(waypoints);
	}

	//決められたルートを移動、巡回。
	public void MoveToWaypoints(bool loop = false)
	{
		if (waypoints.Count == 0) return;

		waypoint = waypoints.Peek();

		if (Vector3.Distance(transform.localPosition, waypoint) > float.Epsilon)
		{
			MoveTo(waypoint);
		}
		else
		{
			if (loop) waypoints.Enqueue(waypoints.Dequeue());
			else waypoints.Dequeue();
		}
	}

	//AIでwaypointを取得して移動。
	public void MoveToWaypoints(State state)
	{
		if (Vector3.Distance(transform.localPosition, waypoint) > float.Epsilon)
		{
			MoveTo(waypoint);
		}
		else
		{
			waypoint = ai.GetWaypoint(Direction, state);
		}
	}

	//目的地まで移動したら、callbackを実行。
	public void MoveToWaypointsThen(Vector2 tp, Action callback)
	{
		if (Vector3.Distance(transform.localPosition, waypoint) > float.Epsilon)
		{
			MoveTo(waypoint);
		}
		else
		{
			if (Vector3.Distance(transform.localPosition, tp) > float.Epsilon)
			{
				waypoint = ai.GetWaypoint(Direction, null, tp);
			}
			else callback();
		}
	}

	//決められたルートで目的地まで移動したら、callbackを実行。
	public void MoveToWaypointsThen(Action callback)
	{
		if (waypoints.Count == 0)
		{
			callback();
			return;
		}

		MoveToWaypoints();
	}

	public void Warp(Vector2 pos)
	{
		transform.localPosition = pos + Direction;
		waypoint = pos + Direction;
	}

	private void MoveTo(Vector2 pos)
	{
		if (pos != (Vector2)transform.localPosition)
		{
			Direction = Vector3.Normalize(pos - (Vector2)transform.localPosition);
		}

		var wp = transform.parent.TransformPoint(pos);
		var p = Vector2.MoveTowards(transform.position, wp, Speed);
		rigid2D.MovePosition(p);
	}
}
