using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create GhostWaypointsData")]
public class GhostWaypointsData : ScriptableObject
{
    public Vector2 Start;
    public List<Vector2> Wait;
    public List<Vector2> Init;
    public List<Vector2> Scatter;
}
