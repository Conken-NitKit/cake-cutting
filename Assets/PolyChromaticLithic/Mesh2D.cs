using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System;

public class Mesh2D
{
    // 頂点座標
    public List<Vector2> vertices { get; set; }

    //切断面の頂点のマージを防ぐため、切断部分を保管し対象外にする
    public HashSet<int> disconnectedVertices { get; set; }

    // 三角形（インデックスのリスト）
    public List<int> triangles { get; set; }

    // UV座標
    public List<Vector2> uv { get; set; }

    // コンストラクタ
    public Mesh2D()
    {
        vertices = new List<Vector2>();
        triangles = new List<int>();
        uv = new List<Vector2>();
        disconnectedVertices = new HashSet<int>();
    }

    // 頂点の追加
    public void AddVertex(Vector2 vertex)
    {
        vertices.Add(vertex);
    }

    // 三角形の追加（頂点インデックスを指定）
    public void AddTriangle(int index0, int index1, int index2)
    {
        triangles.Add(index0);
        triangles.Add(index1);
        triangles.Add(index2);
    }

    // UV座標の追加
    public void AddUV(Vector2 uvCoordinate)
    {
        uv.Add(uvCoordinate);
    }

    // メッシュのリセット
    public void Clear()
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
    }

    // メッシュの作成が完了したかどうかをチェック
    public bool IsValid() => vertices.Count >= 3 && triangles.Count >= 3;
    
    // メッシュのデバッグ用の表示
    public void PrintDebugInfo()
    {
        Debug.Log("Vertices:");
        foreach (var vertex in vertices)
        {
            Debug.Log(vertex);
        }

        Debug.Log("Triangles:");
        for (int i = 0; i < triangles.Count; i += 3)
        {
            Debug.Log($"({triangles[i]}, {triangles[i + 1]}, {triangles[i + 2]})");
        }

        Debug.Log("UVs:");
        foreach (var uvCoordinate in uv)
        {
            Debug.Log(uvCoordinate);
        }
    }

    // UnityのMeshクラスに変換する
    public Mesh ToMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] meshVertices = new Vector3[vertices.Count];
        Color[] colors = new Color[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            meshVertices[i] = new Vector3(vertices[i].x, vertices[i].y, 0);
            colors[i] = disconnectedVertices.Contains(i) ? Color.red : Color.white;
        }
        Debug.Log(String.Join(' ', disconnectedVertices));

        mesh.vertices = meshVertices;
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.colors = colors;
        mesh.RecalculateNormals();

        return mesh;

        Color GetRandomColor()
        {
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }
    }

    public Mesh2D MergeDuplicateVertices()
    {
        //同じレイヤーで同じ位置の頂点をマージする
        //-1は任意のレイヤーとマージできる

        //全ての頂点に対して置換先の頂点を持つDictionaryを作る
        Dictionary<int, int> vertexIndexMap = new();
        List<int> replacedIndexes = new();

        for (int i = 0; i < vertices.Count; i++)
        {
            if (replacedIndexes.Contains(i)) continue;
            for (int j = i + 1; j < vertices.Count; j++)
            {
                if (!disconnectedVertices.Contains(i) && !disconnectedVertices.Contains(j) && vertices[i] == vertices[j])
                {
                    vertexIndexMap[j] = i;
                    replacedIndexes.Add(j);
                }
            }

            vertexIndexMap[i] = i;
        }

        //Dictionaryを使って頂点を置換
        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i] = vertexIndexMap[triangles[i]];
        }

        replacedIndexes.Sort();
        replacedIndexes = replacedIndexes.Distinct().ToList();
        //使用されなくなった頂点を削除する
        for(int i = 0; i < replacedIndexes.Count; i++)
        {
            vertices.RemoveAt(replacedIndexes[i]);
            uv.RemoveAt(replacedIndexes[i]);

            //削除した頂点より後ろの頂点のインデックスを調整
            for (int j = 0; j < triangles.Count; j++)
            {
                if (triangles[j] > replacedIndexes[i])
                {
                    triangles[j]--;
                }
            }
            var disconnectedVerticesTmp = disconnectedVertices.ToList();
            for (int j = 0; j < disconnectedVerticesTmp.Count; j++)
            {
                if (disconnectedVerticesTmp[j] > replacedIndexes[i])
                {
                    disconnectedVertices.Remove(disconnectedVerticesTmp[j]);
                    disconnectedVertices.Add(disconnectedVerticesTmp[j] - 1);
                }
            }

        }

        return this;
    }

    static Mesh2D ToMesh2D(Mesh mesh)
    {
        Mesh2D mesh2D = new Mesh2D();
        Vector3[] meshVertices = mesh.vertices;
        for (int i = 0; i < meshVertices.Length; i++)
        {
            mesh2D.AddVertex(new Vector2(meshVertices[i].x, meshVertices[i].y));
        }
        int[] meshTriangles = mesh.triangles;
        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            mesh2D.AddTriangle(meshTriangles[i], meshTriangles[i + 1], meshTriangles[i + 2]);
        }
        Vector2[] meshUVs = mesh.uv;
        for (int i = 0; i < meshUVs.Length; i++)
        {
            mesh2D.AddUV(meshUVs[i]);
        }
        if(!mesh2D.IsValid())
        {
            Debug.LogError("Invalid mesh2D");
        }
        return mesh2D;
    }
}

