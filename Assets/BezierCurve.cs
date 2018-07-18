using UnityEngine;
using System.Collections;

public class BezierCurve : MonoBehaviour {

    public Vector2[] points;

    public void Reset()
    {
        points = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(2, 0),
            new Vector2(3, 0),
            new Vector2(4, 0)
        };
    }

    public Vector2 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], t));
    }

}
