using UnityEngine;
using System.Collections;
using System;

public class BezierSpline : MonoBehaviour {

    [SerializeField]
    private Vector2[] points;

    [SerializeField]
    private BezierControlPointMode[] modes;

    public void Start()
    {
        
    }

    public void Reset()
    {
        points = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(2, 0),
            new Vector2(3, 0),
            new Vector2(4, 0)
        };

        modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    public int ControlPointCount
    {
        get
        {
            return points.Length;
        }
    }

    public Vector2 GetControlPoint(int index)
    {
        return points[index];
    }

    public void SetControlPoint(int index, Vector2 point) {
        if (index % 3 == 0)
        {
            Vector2 delta = point - points[index];
            if (index > 0)
            {
                points[index - 1] += delta;
            }
            if (index + 1 < points.Length)
            {
                points[index + 1] += delta;
            }
        }
        points[index] = point;
        EnforceMode(index);
    }

    public Vector2 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], t));
    }

    public Vector2[] GetPoints()
    {
        return points;
    }

    public void AddCurve(int after)
    {
        Vector2[] newPoints = new Vector2[points.Length + 3];
        BezierControlPointMode[] newModes = new BezierControlPointMode[modes.Length + 1];

        for (int i = 0; i < newPoints.Length; i++)
        {
            if (i <= after)
            {
                newPoints[i] = points[i];
            } else if (i <= after + 3)
            {
                if (after < 0)
                {
                    newPoints[i] = points[0] + (points[0] - points[1]).normalized * 5 * (float) Math.Pow(-1, after-i);
                }
                else if (after == points.Length - 1)
                {
                    newPoints[i] = points[points.Length-1] + (points[points.Length-2] - points[points.Length-1]).normalized * 5 * (float)Math.Pow(-1, after - i);
                } else
                {
                    newPoints[i] = Bezier.GetPoint(points[after], points[after + 1], points[after + 2], points[after + 3], 0.5f);
                }
            } else
            {
                newPoints[i] = points[i - 3];
            }
        }

        for (int i = 0; i < newModes.Length; i++)
        {
            if (i <= after/3)
            {
                newModes[i] = modes[i];
            } else if (i <= after/3 + 1)
            {
                if (after >= 0 && after < points.Length-1)
                {
                    newModes[i] = BezierControlPointMode.Mirrored;
                } else
                {
                    newModes[i] = BezierControlPointMode.Free;
                }
            } else
            {
                newModes[i] = modes[i - 1];
            }
        }

        points = newPoints;
        modes = newModes;

        for (int i = 0; i < points.Length; i++)
        {
            EnforceMode(i);
        }
    }

    public void RemoveCurve(int index)
    {
        if (ControlPointCount <= 4)
        {
            return;
        }

        Vector2[] newPoints = new Vector2[points.Length - 3];
        BezierControlPointMode[] newModes = new BezierControlPointMode[modes.Length - 1];

        int skipped = 0;
        for (int i = 0; i < points.Length; i++)
        {
            int ci = i - skipped;
            if (i != index && i-1 != index && i+1 != index)
            {
                newPoints[ci] = points[i];
                if (i % 3 == 0)
                {
                    newModes[i / 3] = modes[(i / 3)];
                }
            } else
            {
                skipped++;
            }
        }

        points = newPoints;
        modes = newModes;
    }

    public BezierControlPointMode GetControlPointMode(int index)
    {
        return modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        modes[(index + 1) / 3] = mode;
        EnforceMode(index);
    }

    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Length - 1)
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;

        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            enforcedIndex = middleIndex + 1;
        } else
        {
            fixedIndex = middleIndex + 1;
            enforcedIndex = middleIndex - 1;
        }

        Vector2 middle = points[middleIndex];
        Vector2 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector2.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }
}
