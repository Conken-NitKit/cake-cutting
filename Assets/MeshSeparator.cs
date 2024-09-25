using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MeshSeparator
{
    static public Mesh2D[] SeparateMesh(Mesh2D mesh2d)
    {
        // 各部分のメッシュを保存するリスト
        List<Mesh2D> separatedMeshes = new List<Mesh2D>();

        // 各頂点の接続状態をチェックするためのリスト
        List<int> visited = new List<int>();

        // 頂点リストを取得
        Vector2[] vertices = mesh2d.vertices.ToArray();
        int[] triangles = mesh2d.triangles.ToArray();
        Vector2[] uvs = mesh2d.uv.ToArray();

        // 各頂点について処理を行う
        for (int i = 0; i < vertices.Length; i++)
        {
            if (!visited.Contains(i))
            {
                // 繋がっている部分を探して新しいメッシュを作成
                List<int> connectedVertices = new List<int>();
                List<int> connectedTriangles = new List<int>();

                FindConnectedVertices(i, vertices, triangles, visited, connectedVertices, connectedTriangles);

                if (connectedVertices.Count < 3)
                {
                    continue;
                }

                // 新しいメッシュを作成
                Mesh2D newMesh = CreateMesh(vertices, connectedTriangles.ToArray(), uvs);
                separatedMeshes.Add(newMesh);
            }
        }

        return separatedMeshes.ToArray();


    }

    static public Mesh2D[] SeparateMeshes(params Mesh2D[] mesh2D)
    {
        List<Mesh2D> separatedMeshes = new List<Mesh2D>();
        foreach (Mesh2D mesh in mesh2D)
        {
            Mesh2D[] meshes = SeparateMesh(mesh);
            separatedMeshes.AddRange(meshes);
        }
        return separatedMeshes.ToArray();
    }

    static private void FindConnectedVertices(int startIndex, Vector2[] vertices, int[] triangles, List<int> visited, List<int> connectedVertices, List<int> connectedTriangles)
    {
        Stack<int> stack = new Stack<int>();
        stack.Push(startIndex);
        visited.Add(startIndex);

        while (stack.Count > 0)
        {
            int currentIndex = stack.Pop();
            connectedVertices.Add(currentIndex);

            // 頂点に接続しているすべての三角形を探す
            for (int i = 0; i < triangles.Length; i += 3)
            {
                if (triangles[i] == currentIndex || triangles[i + 1] == currentIndex || triangles[i + 2] == currentIndex)
                {
                    connectedTriangles.Add(triangles[i]);
                    connectedTriangles.Add(triangles[i + 1]);
                    connectedTriangles.Add(triangles[i + 2]);

                    // 三角形の他の頂点をスタックに追加
                    for (int j = 0; j < 3; j++)
                    {
                        if (!visited.Contains(triangles[i + j]))
                        {
                            stack.Push(triangles[i + j]);
                            visited.Add(triangles[i + j]);
                        }
                    }
                }
            }
        }
    }

    static private Mesh2D CreateMesh(Vector2[] vertices, int[] triangles, Vector2[] uvs)
    {
        Mesh2D newMesh = new Mesh2D();
        newMesh.vertices = vertices.ToList();
        newMesh.triangles = triangles.ToList();
        newMesh.uv = uvs.ToList();
        return newMesh;
    }
}

