using UnityEngine;
using System.Collections;

public static class Bezier
{
    public static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float ct = 1 - t;
        return p0 * (ct * ct) + p1 * (2 * ct * t) + p2 * (t * t);
    }

    public static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float ct = 1 - t;
        return p0 * (ct * ct * ct) + p1 * (3 * ct * ct * t) + p2 * (3 * ct * t * t) + p3 * (t * t * t);
    }
}
