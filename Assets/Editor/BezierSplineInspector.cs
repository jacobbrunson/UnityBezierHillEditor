using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int lineSteps = 10;

    private int selectedIndex = -1;

    private static Color[] modeColors =
    {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    public override void OnInspectorGUI()
    {
        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
        {
            DrawSelectedPointInspector();
        }
        spline = target as BezierSpline;

        int mod = selectedIndex % 3;

        GUILayout.Label("Add New Curve");

        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

        if (GUILayout.Button("Beginning"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve(-1);
            EditorUtility.SetDirty(spline);
        }

        if (selectedIndex != -1)
        {
            if (GUILayout.Button("Before"))
            {
                Undo.RecordObject(spline, "Add Curve");
                if (mod == 0)
                {
                    spline.AddCurve(selectedIndex - 3);
                } else if (mod == 1)
                {
                    spline.AddCurve(selectedIndex - 4);
                } else if (mod == 2)
                {
                    spline.AddCurve(selectedIndex - 2);
                }
                EditorUtility.SetDirty(spline);
            }

            if (GUILayout.Button("After"))
            {
                Undo.RecordObject(spline, "Add Curve");
                if (mod == 0)
                {
                    spline.AddCurve(selectedIndex);
                }
                else if (mod == 1)
                {
                    spline.AddCurve(selectedIndex - 1);
                }
                else if (mod == 2)
                {
                    spline.AddCurve(selectedIndex + 1);
                }
                EditorUtility.SetDirty(spline);
            }
        }

        if (GUILayout.Button("End"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve(spline.ControlPointCount - 1);
            EditorUtility.SetDirty(spline);
        }

        GUILayout.EndHorizontal();
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");

        EditorGUI.BeginChangeCheck();
        Vector2 point = EditorGUILayout.Vector2Field("Position", spline.GetControlPoint(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, point);
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            if (selectedIndex > 1 && selectedIndex < spline.ControlPointCount - 2)
            {
                Undo.RecordObject(spline, "Change Point Mode");
                spline.SetControlPointMode(selectedIndex, mode);
                EditorUtility.SetDirty(spline);
            }
        }
    }

    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        Event e = Event.current;
        switch (e.type)
        {
            case EventType.KeyDown:
                if (Event.current.keyCode == KeyCode.Delete && selectedIndex != -1)
                {
                    if (selectedIndex % 3 == 0)
                    {
                        spline.RemoveCurve(selectedIndex);
                    }
                    Event.current.Use();
                }
                break;
        }

        Vector2 p0 = ShowPoint(0);
        for (int i = 1; i < spline.ControlPointCount; i += 3)
        {
            Vector2 p1 = ShowPoint(i);
            Vector2 p2 = ShowPoint(i + 1);
            Vector2 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2);
            p0 = p3;
        }

    }

    private Vector2 ShowPoint(int index)
    {
        Vector2 point = handleTransform.TransformPoint(spline.GetControlPoint(index));

        float size = HandleUtility.GetHandleSize(point);
        Color color = modeColors[(int)spline.GetControlPointMode(index)];

        Handles.color = color;
        MyHandles.DragHandleResult dhResult;
        Vector2 newPosition = MyHandles.DragHandle(point, 0.04f * size, Handles.DotCap, color, out dhResult);

        switch (dhResult)
        {
            case MyHandles.DragHandleResult.LMBDrag:
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(newPosition));
                selectedIndex = index;
                Repaint();
                break;
            case MyHandles.DragHandleResult.LMBClick:
                if (selectedIndex == index)
                {
                    selectedIndex = -1;
                } else
                {
                    selectedIndex = index;
                }
                Repaint();
                break;
        }

        if (selectedIndex == index)
        {
            Handles.color = Color.red;
            Handles.CircleCap(0, point, Quaternion.identity, 0.3f * size);
        }
        return point;
    }
}
