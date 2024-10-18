using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Mesh2D
{

    // ���_���W
    public List<Vector2> vertices { get; set; }

    // �O�p�`�i�C���f�b�N�X�̃��X�g�j
    public List<int> triangles { get; set; }

    // UV���W
    public List<Vector2> uv { get; set; }

    public List<Color> colors { get; set; }

    // �R���X�g���N�^
    public Mesh2D()
    {
        vertices = new List<Vector2>();
        triangles = new List<int>();
        uv = new List<Vector2>();
        colors = new List<Color>();
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

    // �F�̒ǉ�
    public void AddColor(Color color)
    {
        colors.Add(color);
    }

    // ���b�V���̃��Z�b�g
    public void Clear()
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
        colors.Clear();
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
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    public Mesh2D MergeDuplicateVertices()
    {
        //�������C���[�œ����ʒu�̒��_���}�[�W����
        //-1�͔C�ӂ̃��C���[�ƃ}�[�W�ł���

        //�S�Ă̒��_�ɑ΂��Ēu����̒��_������Dictionary�����
        Dictionary<int, int> vertexIndexMap = new();
        List<int> replacedIndexes = new();

        for (int i = 0; i < vertices.Count; i++)
        {
            if (replacedIndexes.Contains(i)) continue;
            for (int j = i + 1; j < vertices.Count; j++)
            {
                if (vertices[i] == vertices[j])
                {
                    vertexIndexMap[j] = i;
                    replacedIndexes.Add(j);
                }
            }

            vertexIndexMap[i] = i;
        }

        //Dictionary���g���Ē��_��u��
        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i] = vertexIndexMap[triangles[i]];
        }

        replacedIndexes.Sort();
        replacedIndexes = replacedIndexes.Distinct().ToList();
        //�g�p����Ȃ��Ȃ������_���폜����
        for (int i = 0; i < replacedIndexes.Count; i++)
        {
            vertices.RemoveAt(replacedIndexes[i]);
            uv.RemoveAt(replacedIndexes[i]);
            colors.RemoveAt(replacedIndexes[i]);

            //�폜�������_�����̒��_�̃C���f�b�N�X�𒲐�
            for (int j = 0; j < triangles.Count; j++)
            {
                if (triangles[j] > replacedIndexes[i])
                {
                    triangles[j]--;
                }
            }

        }

        return this;
    }

    public static void ReverseFaces(Mesh mesh)
    {
        for (int i = 0; i < mesh.triangles.Count(); i += 3)
        {
            (mesh.triangles[i + 1], mesh.triangles[i]) = (mesh.triangles[i], mesh.triangles[i + 1]);
        }
        //mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public Mesh2D NormalizeSize()
    {
        float minX = vertices.Min(v => v.x);
        float minY = vertices.Min(v => v.y);
        float maxX = vertices.Max(v => v.x);
        float maxY = vertices.Max(v => v.y);
        float width = maxX - minX;
        float height = maxY - minY;
        float scale = 10.0f / Mathf.Max(width, height);
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = new Vector2((vertices[i].x - minX) * scale, (vertices[i].y - minY) * scale);
        }
        return this;
    }

    public float CalcurateArea()
    {
        float area = 0;
        for (int i = 0; i < triangles.Count; i += 3)
        {
            Vector2 a = vertices[triangles[i]];
            Vector2 b = vertices[triangles[i + 1]];
            Vector2 c = vertices[triangles[i + 2]];
            area += Mathf.Abs((a.x * b.y + b.x * c.y + c.x * a.y - a.x * c.y - b.x * a.y - c.x * b.y) / 2);
        }
        return area;
    }

    public Vector2 CalcurateCentroid()
    {
        float area = CalcurateArea();
        float x = 0;
        float y = 0;
        for (int i = 0; i < triangles.Count; i += 3)
        {
            Vector2 a = vertices[triangles[i]];
            Vector2 b = vertices[triangles[i + 1]];
            Vector2 c = vertices[triangles[i + 2]];
            x += (a.x + b.x + c.x) * (a.x * b.y + b.x * c.y + c.x * a.y - a.x * c.y - b.x * a.y - c.x * b.y) / 6;
            y += (a.y + b.y + c.y) * (a.x * b.y + b.x * c.y + c.x * a.y - a.x * c.y - b.x * a.y - c.x * b.y) / 6;
        }
        return new Vector2(x / area, y / area);
    }

    public static Mesh2D ToMesh2D(Mesh mesh)
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
        Color[] meshColors = mesh.colors;
        if (meshColors.Length < meshVertices.Length)
        {
            for (int i = 0; i < meshVertices.Length - meshColors.Length; i++)
            {
                mesh2D.AddColor(Color.white);
            }
        }
        else
        {
            for (int i = 0; i < meshColors.Length; i++)
            {
                mesh2D.AddColor(meshColors[i]);
            }
        }
        if (!mesh2D.IsValid())
        {
            Debug.LogError("Invalid mesh2D");
        }
        return mesh2D;
    }

}

