using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MeshSeparator
{
    static public Mesh2D[] SeparateMesh(Mesh2D mesh2d)
    {
        // �e�����̃��b�V����ۑ����郊�X�g
        List<Mesh2D> separatedMeshes = new List<Mesh2D>();

        // �e���_�̐ڑ���Ԃ��`�F�b�N���邽�߂̃��X�g
        List<int> visited = new List<int>();

        // ���_���X�g���擾
        Vector2[] vertices = mesh2d.vertices.ToArray();
        int[] triangles = mesh2d.triangles.ToArray();

        // �e���_�ɂ��ď������s��
        for (int i = 0; i < vertices.Length; i++)
        {
            if (!visited.Contains(i))
            {
                // �q�����Ă��镔����T���ĐV�������b�V�����쐬
                List<int> connectedVertices = new List<int>();
                List<int> connectedTriangles = new List<int>();

                FindConnectedVertices(i, vertices, triangles, visited, connectedVertices, connectedTriangles);

                // �V�������b�V�����쐬
                Mesh2D newMesh = CreateMesh(vertices, connectedTriangles.ToArray());
                separatedMeshes.Add(newMesh);
            }
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

            // ���_�ɐڑ����Ă��邷�ׂĂ̎O�p�`��T��
            for (int i = 0; i < triangles.Length; i += 3)
            {
                if (triangles[i] == currentIndex || triangles[i + 1] == currentIndex || triangles[i + 2] == currentIndex)
                {
                    connectedTriangles.Add(triangles[i]);
                    connectedTriangles.Add(triangles[i + 1]);
                    connectedTriangles.Add(triangles[i + 2]);

                    // �O�p�`�̑��̒��_���X�^�b�N�ɒǉ�
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

    static private Mesh2D CreateMesh(Vector2[] vertices, int[] triangles)
    {
        Mesh2D newMesh = new Mesh2D();
        Vector2[] newVertices = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            newVertices[i] = vertices[i];
        }
        newMesh.vertices = newVertices.ToList();
        newMesh.triangles = triangles.ToList();
        return newMesh;
    }
}

