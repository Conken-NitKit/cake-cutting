using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    public int segments = 36;  // 円の分割数（頂点の数）
    public float radius = 1f;  // 円の半径

    void Start()
    {
        this.GetComponent<Mesh2DAssigner>().Mesh2D = GenerateDonutMesh();
    }

    Mesh2D GenerateCircleMesh()
    {
        
        Mesh2D mesh2D = new Mesh2D();

        Vector2[] vertices = new Vector2[segments + 1];
        int[] triangles = new int[segments * 3];
        Vector2[] uv = new Vector2[segments + 1];

        // 中心点
        vertices[0] = Vector2.zero;
        uv[0] = new Vector2(0.5f, 0.5f);

        // 頂点を生成
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            vertices[i + 1] = new Vector3(x, y, 0);
            uv[i + 1] = new Vector2((x / radius + 1) * 0.5f, (y / radius + 1) * 0.5f);

            // 三角形を設定
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 1 == segments ? 1 : i + 2;
        }

        // メッシュに頂点、三角形、UVを設定
        mesh2D.vertices = vertices.ToList();
        mesh2D.triangles = triangles.ToList();
        mesh2D.uv = uv.ToList();

        return mesh2D;
    }

    Mesh2D GenerateDonutMesh()
    {
        Mesh2D mesh2D = new Mesh2D();
        
        Vector2[] vertices = new Vector2[segments * 2];
        int[] triangles = new int[segments * 6];
        Vector2[] uv = new Vector2[segments * 2];

        float angleStep = 360.0f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);

            vertices[i * 2] = new Vector3(x * (radius + 1), y * (radius + 1), 0);
            vertices[i * 2 + 1] = new Vector3(x * (radius / 1.5f), y * (radius / 1.5f), 0);

            uv[i * 2] = new Vector2((float)i / segments, 1);
            uv[i * 2 + 1] = new Vector2((float)i / segments, 0);

            int triIndex = i * 6;
            triangles[triIndex] = i * 2;
            triangles[triIndex + 1] = (i * 2 + 2) % (segments * 2);
            triangles[triIndex + 2] = i * 2 + 1;

            triangles[triIndex + 3] = i * 2 + 1;
            triangles[triIndex + 4] = (i * 2 + 2) % (segments * 2);
            triangles[triIndex + 5] = (i * 2 + 3) % (segments * 2);
        }

        mesh2D.vertices = vertices.ToList();
        mesh2D.triangles = triangles.ToList();
        mesh2D.uv = uv.ToList();

        return mesh2D;

    }

    Mesh2D GenerateSquareMesh()
    {
        Mesh2D mesh2D = new Mesh2D();

        mesh2D.vertices = new List<Vector2>() 
        {
            new Vector2(-1, -1),
            new Vector2(1, -1),
            new Vector2(1, 1),
            new Vector2(-1, 1)
        };
        mesh2D.triangles = new List<int>() 
        {
            0, 1, 2,
            0, 2, 3
        };
        mesh2D.uv = new List<Vector2>()
{
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
        
        return mesh2D;


    }

    Mesh2D GenerateTriangleMesh()
    {
        Mesh2D mesh2D = new Mesh2D();
        mesh2D.vertices = new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1)
        };
        mesh2D.triangles = new List<int>()
        {
            0, 1, 2
        };
        mesh2D.uv = new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1)
        };
        return mesh2D;
    }
}
