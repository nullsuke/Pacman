using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    [SerializeField] private Vector2 warpOut = default;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var w = collision.GetComponent<IWarpable>();
        w.Warp(warpOut);
    }
}
