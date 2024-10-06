using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System;

public class Mesh2D
{
    // ���_���W
    public List<Vector2> vertices { get; set; }

    // �ؒf�ʂ̒��_�̃}�[�W��h�����߁A�ؒf������ۊǂ��ΏۊO�ɂ���
    public HashSet<Vector2> disconnectedVertices { get; set; }

    // Key�͐ڍ����Ȃ����_�̍��W�A�ڍ����Ȃ����_��*���̒��_*��Normalized�����x�N�g��
    public Dictionary<Vector2,Vector2> blackLists { get; set; }

    // blackLists�̌v�Z���ׂ��팸���邽�߁A������

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
        disconnectedVertices = new HashSet<Vector2>();
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
        Color[] colors = new Color[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            meshVertices[i] = new Vector3(vertices[i].x, vertices[i].y, 0);
            //colors[i] = disconnectedVertices.Contains(vertices[i]) ? Color.red : Color.white;
            colors[i] = GetRandomColor();
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
                if (!disconnectedVertices.Contains(vertices[i]) && !disconnectedVertices.Contains(vertices[j]) && vertices[i] == vertices[j])
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
        for(int i = 0; i < replacedIndexes.Count; i++)
        {
            vertices.RemoveAt(replacedIndexes[i]);
            uv.RemoveAt(replacedIndexes[i]);

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

    public Mesh2D ReverseFaces()
    {
        for (int i = 0; i < triangles.Count; i += 3)
        {
            (triangles[i + 1], triangles[i]) = (triangles[i], triangles[i + 1]);
        }
        return this;
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

    static public Mesh2D ToMesh2D(Mesh mesh)
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

