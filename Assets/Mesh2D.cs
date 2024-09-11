using UnityEngine;
using System.Collections.Generic;

public class Mesh2D
{
    // ���_���W
    public List<Vector2> vertices { get; set; }

    // �O�p�`�i�C���f�b�N�X�̃��X�g�j
    public List<int> triangles { get; set; }

    // UV���W
    public List<Vector2> uv { get; set; }

    // �R���X�g���N�^
    public Mesh2D()
    {
        vertices = new List<Vector2>();
        triangles = new List<int>();
        uv = new List<Vector2>();
    }

    // ���_�̒ǉ�
    public void AddVertex(Vector2 vertex)
    {
        vertices.Add(vertex);
    }

    // �O�p�`�̒ǉ��i���_�C���f�b�N�X���w��j
    public void AddTriangle(int index0, int index1, int index2)
    {
        triangles.Add(index0);
        triangles.Add(index1);
        triangles.Add(index2);
    }

    // UV���W�̒ǉ�
    public void AddUV(Vector2 uvCoordinate)
    {
        uv.Add(uvCoordinate);
    }

    // ���b�V���̃��Z�b�g
    public void Clear()
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
    }

    // ���b�V���̍쐬�������������ǂ������`�F�b�N
    public bool IsValid() => vertices.Count >= 3 && triangles.Count >= 3;
    
    // ���b�V���̃f�o�b�O�p�̕\��
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

    // Unity��Mesh�N���X�ɕϊ�����
    public Mesh ToMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] meshVertices = new Vector3[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            meshVertices[i] = new Vector3(vertices[i].x, vertices[i].y, 0);
        }

        mesh.vertices = meshVertices;
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();

        return mesh;
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

