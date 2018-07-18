using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {

    public int resolution = 1;

    void Start () {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;

        BezierSpline spline = GetComponent<BezierSpline>();
        

        //Vector2[] uv =
        //;

        //int[] 

        Vector2[] points = spline.GetPoints();

        EdgeCollider2D ec = gameObject.AddComponent<EdgeCollider2D>();
        Vector2[] edgePoints = new Vector2[points.Length / 3 * resolution + 1];

        for (int i = 0; i < points.Length - 1; i += 3)
        {
            for (int j = 0; j < resolution; j++)
            {
                edgePoints[i / 3 * resolution + j] = Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], j / (float)resolution);
            }
        }

        edgePoints[edgePoints.Length - 1] = points[points.Length - 1];

        ec.points = edgePoints;


        Debug.Log(edgePoints.Length);


        Vector3[] verts = new Vector3[edgePoints.Length * 3];
        Vector2[] uv = new Vector2[edgePoints.Length * 3];
        int[] triangles = new int[(edgePoints.Length - 1) * 12];


        for (int i = 0; i < edgePoints.Length; i++)
        {
            Vector2 point = edgePoints[i];
            verts[i * 3 + 0] = new Vector3(point.x, point.y, -1);
            verts[i * 3 + 1] = new Vector3(point.x, point.y, 1);
            verts[i * 3 + 2] = new Vector3(point.x, -100, 0);

            uv[i * 3 + 0] = new Vector2(0, 0);
            uv[i * 3 + 1] = new Vector2(0, 0);
            uv[i * 3 + 2] = new Vector2(0, 0);

            if (i > 0)
            {
                int j = i - 1;
                triangles[j * 12 +  0] = j * 3 + 0;
                triangles[j * 12 +  1] = j * 3 + 1;
                triangles[j * 12 +  2] = j * 3 + 3;

                triangles[j * 12 +  3] = j * 3 + 1;
                triangles[j * 12 +  4] = j * 3 + 4;
                triangles[j * 12 +  5] = j * 3 + 3;

                triangles[j * 12 +  6] = j * 3 + 2;
                triangles[j * 12 +  7] = j * 3 + 0;
                triangles[j * 12 +  8] = j * 3 + 5;

                triangles[j * 12 +  9] = j * 3 + 0;
                triangles[j * 12 + 10] = j * 3 + 3;
                triangles[j * 12 + 11] = j * 3 + 5;
            }
        }

        for (int i = 0; i < triangles.Length; i++)
        {
            Debug.Log(triangles[i]);
        }

        //verts = new Vector3[]
        //{
        //    new Vector3(0, 0, 0),
        //    new Vector3(1, 0, 0),
        //    new Vector3(1, 0, 1),
        //    new Vector3(0, 0, 1),
        //};

        //triangles = new int[]{
        //    0, 1, 2,
        //    1, 3, 2
        //};

        mesh.Clear();

        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
