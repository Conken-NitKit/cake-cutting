using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PolygonCutter
{
    private class MeshCutSide
    {
        public List<(Vector2 position, int layer)> vertices = new();
        public List<int> triangles = new();
        public List<Vector2> uvs = new();
        public HashSet<Vector2> disconnecteds = new();

        public void ClearAll()
        {
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
            disconnecteds.Clear();
        }

        //���̒��_���Q�Ƃ��ĎO�p�`��ǉ�
        //public void AddTriangle(int index1, int index2, int index3, int layer, bool forceClockWise = true)
        //{
        //    var nowIndex = vertices.Count;
        //    if (forceClockWise)
        //    {
        //        if (IsClockWise(victimMesh.vertices[index1], victimMesh.vertices[index2], victimMesh.vertices[index3]))
        //        {
        //            (index3, index2) = (index2, index3);
        //            triangles.Add(nowIndex + 0);
        //            triangles.Add(nowIndex + 2);
        //            triangles.Add(nowIndex + 1);

        //        }
        //        else
        //        {
        //            triangles.Add(nowIndex + 0);
        //            triangles.Add(nowIndex + 1);
        //            triangles.Add(nowIndex + 2);

        //        }
        //    }
        //    else
        //    {
        //        triangles.Add(nowIndex + 2);
        //        triangles.Add(nowIndex + 1);
        //        triangles.Add(nowIndex + 0);
        //    }

        //    vertices.Add((victimMesh.vertices[index1], layer));
        //    vertices.Add((victimMesh.vertices[index2], layer));
        //    vertices.Add((victimMesh.vertices[index3], layer));

        //    uvs.Add(victimMesh.uv[index1]);
        //    uvs.Add(victimMesh.uv[index2]);
        //    uvs.Add(victimMesh.uv[index3]);

        //}

        public void AddTriangle(int index1, int index2, int index3, int layer1, int layer2, int layer3, bool forceClockWise = true)
        {
            var nowIndex = vertices.Count;
            if (forceClockWise)
            {
                if (IsClockWise(victimMesh.vertices[index1], victimMesh.vertices[index2], victimMesh.vertices[index3]))
                {
                    (index3, index2) = (index2, index3);
                    (layer3, layer2) = (layer2, layer3);
                    triangles.Add(nowIndex + 0);
                    triangles.Add(nowIndex + 2);
                    triangles.Add(nowIndex + 1);

                }
                else
                {
                    triangles.Add(nowIndex + 0);
                    triangles.Add(nowIndex + 1);
                    triangles.Add(nowIndex + 2);

                }
            }
            else
            {
                triangles.Add(nowIndex + 2);
                triangles.Add(nowIndex + 1);
                triangles.Add(nowIndex + 0);
            }

            vertices.Add((OffsetVector(victimMesh.vertices[index1], layer1), disconnecteds.Contains(victimMesh.vertices[index1]) ? -4 : layer1));
            vertices.Add((OffsetVector(victimMesh.vertices[index2], layer2), disconnecteds.Contains(victimMesh.vertices[index2]) ? -4 : layer2));
            vertices.Add((OffsetVector(victimMesh.vertices[index3], layer3), disconnecteds.Contains(victimMesh.vertices[index3]) ? -4 : layer3));


            uvs.Add(victimMesh.uv[index1]);
            uvs.Add(victimMesh.uv[index2]);
            uvs.Add(victimMesh.uv[index3]);

        }


        public void AddTriangle(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 uv1, Vector2 uv2, Vector2 uv3, int layer1, int layer2, int layer3, bool forceClockWise = true)
        {

            var nowIndex = vertices.Count;
            if (forceClockWise && IsClockWise(point1, point2, point3))
            {
                (point3, point2) = (point2, point3);
                (uv3, uv2) = (uv2, uv3);
                (layer3, layer2) = (layer2, layer3);

                triangles.Add(nowIndex + 2);
                triangles.Add(nowIndex + 0);
                triangles.Add(nowIndex + 1);

            }
            else
            {

                triangles.Add(nowIndex + 2);
                triangles.Add(nowIndex + 1);
                triangles.Add(nowIndex + 0);
            }


            vertices.Add((OffsetVector(point1, layer1), layer1));
            vertices.Add((OffsetVector(point2, layer2), layer1));
            vertices.Add((OffsetVector(point3, layer3), layer3));

            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);

        }

        private bool isIn45_135;

        public void SetStartPointAndEndPoint(Vector2 startPoint, Vector2 endPoint)
        {
            isIn45_135 = IsClockWise(new Vector2(0, 0), new Vector2(1, 1), endPoint - startPoint);
        }

        public void AddDisconnectedPoint(Vector2 point, int layer)
        {

        }

        private Vector2 OffsetVector(Vector2 point, int layer)
        {
            var ans = point;
            if (layer == -1) return ans;
            if (isIn45_135)
            {
                if (layer == 0)
                {
                    ans = new Vector2(BitIncrement(ans.x), BitIncrement(ans.y));
                    //ans += new Vector2(0.1f, 0.1f);
                }
                else
                {
                    ans = new Vector2(BitDecrement(ans.x), BitDecrement(ans.y));
                   // ans -= new Vector2(0.1f, 0.1f);
                }
            }
            else
            {
                if (layer == 0)
                {
                    ans = new Vector2(BitDecrement(ans.x), BitDecrement(ans.y));
                   // ans -= new Vector2(0.1f, 0.1f);
                }
                else
                {
                    ans = new Vector2(BitIncrement(ans.x), BitIncrement(ans.y));
                   // ans += new Vector2(0.1f, 0.1f);
                }
            }
            return ans;

        }

        public Mesh2D ToMesh2D()
        {
            var mesh = new Mesh2D();
            mesh.vertices = vertices.Select(x => x.position).ToList();
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.disconnectedVertices = new(disconnecteds);
            return mesh;
        }

        public void Combine(MeshCutSide meshCutSide)
        {
            var nowIndex = vertices.Count;
            vertices.AddRange(meshCutSide.vertices);
            foreach (var triangle in meshCutSide.triangles)
            {
                triangles.Add(triangle + nowIndex);
            }
            uvs.AddRange(meshCutSide.uvs);
            foreach (var disconnected in meshCutSide.disconnecteds)
            {
                disconnecteds.Add(disconnected);
            }
        }

        public void MergeDuplicateVertices(HashSet<Vector2> disconnecteds)
        {
            //�������C���[�œ����ʒu�̒��_���}�[�W����
            //-1��-4�ȊO�̔C�ӂ̃��C���[�ƃ}�[�W�ł���
            //-4�͈�؃}�[�W����Ȃ�
            //���̃}�W�b�N�i���o�[�̎����͍ň��Ȃ̂ł����ǂ��ɂ�������

            //�S�Ă̒��_�ɑ΂��Ēu����̒��_������Dictionary�����
            Dictionary<int, int> vertexIndexMap = new();
            Dictionary<int, int> layerMap = new();
            List<int> replacedIndexes = new();

            for (int i = 0; i < vertices.Count; i++)
            {
                if (replacedIndexes.Contains(i)) continue;
                for (int j = i + 1; j < vertices.Count; j++)
                {
                    //if (vertices[i].layer == -4 || vertices[j].layer == -4) continue;
                    if (vertices[i].position.Equals(vertices[j].position) /* && (vertices[i].layer == -1 || vertices[j].layer == -1 || vertices[i].layer == vertices[j].layer)*/)
                    {
                        vertexIndexMap[j] = i;
                        int layer = -1;
                        if (vertices[i].layer != -1) layer = vertices[i].layer;
                        if (vertices[j].layer != -1) layer = vertices[j].layer;
                        layerMap[i] = layer;
                        replacedIndexes.Add(j);
                    }
                }

                layerMap[i] = vertices[i].layer;
                vertexIndexMap[i] = i;
            }

            replacedIndexes.Sort();
            replacedIndexes = replacedIndexes.Distinct().ToList();
            //Dictionary���g���Ē��_��u��
            for (int i = 0; i < triangles.Count; i++)
            {
                triangles[i] = vertexIndexMap[triangles[i]];
            }

            //�g�p����Ȃ��Ȃ������_���폜����
            var trianglesTmp = triangles.ToList();
            for (int i = replacedIndexes.Count - 1; i >= 0; i--)
            {
                vertices.RemoveAt(replacedIndexes[i]);
                uvs.RemoveAt(replacedIndexes[i]);


                //�폜�������_�����̒��_�̃C���f�b�N�X�𒲐�
                for (int j = 0; j < triangles.Count; j++)
                {
                    if (trianglesTmp[j] > replacedIndexes[i])
                    {
                        triangles[j]--;
                    }
                }

            }

            //���C���[�����X�V
            for (int i = 0; i < vertices.Count; i++)
            {
                if (layerMap.ContainsKey(i))
                {
                    vertices[i] = (vertices[i].position, layerMap[i]);
                }
            }
        }

    }



    private static Mesh2D victimMesh;
    private static MeshCutSide cwSide;
    private static MeshCutSide acwSide;

     public static Mesh2D[] SegmentCut(Mesh2D victim, Vector2 startPoint, Vector2 endPoint)
    {
        victimMesh = victim;
        //�ؒf���钼���ɑ΂����v���(clockwise)���Ɣ����v���(anti-clockwise)���ɕ�����
        cwSide = new MeshCutSide();
        cwSide.SetStartPointAndEndPoint(startPoint, endPoint);
        acwSide = new MeshCutSide();
        acwSide.SetStartPointAndEndPoint(startPoint, endPoint);
        cwSide.disconnecteds = victimMesh.disconnectedVertices;
        acwSide.disconnecteds = victimMesh.disconnectedVertices;

        bool isConnecting = false;
        //�O�p�`��U�蕪����
        for (int triangleCount = 0; triangleCount < victim.triangles.Count; triangleCount += 3)
        {
            var index1 = victim.triangles[triangleCount];
            var index2 = victim.triangles[triangleCount + 1];
            var index3 = victim.triangles[triangleCount + 2];
            if (SegmentCutTriangle(index1, index2, index3, startPoint, endPoint)) isConnecting = true;
        }
        Debug.Log("cwSide0:" + string.Join(",", cwSide.vertices));
        if (isConnecting)
        {
            cwSide.Combine(acwSide);
        }
        Debug.Log("cwSide:" + string.Join(",", cwSide.vertices));
        cwSide.MergeDuplicateVertices(victimMesh.disconnectedVertices);
        Debug.Log("cwSide2:" + string.Join(",", cwSide.vertices));
        acwSide.MergeDuplicateVertices(victimMesh.disconnectedVertices);
        var cwMesh = cwSide.ToMesh2D();
        var acwMesh = acwSide.ToMesh2D();
        if (isConnecting)
        {
            //return new Mesh2D[] { cwMesh };
            return MeshSeparator.SeparateMesh(cwMesh);
        }
        else
        {
            //return new Mesh2D[] { cwMesh, acwMesh };
            return MeshSeparator.SeparateMeshes(cwMesh, acwMesh);
        }

    }

 

    /// <summary>
    /// �ؒf�ʂ��������Ă���ꍇ�ATrue��Ԃ��܂�
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    /// <param name="index3"></param>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    private static bool SegmentCutTriangle(int index1, int index2, int index3, Vector2 startPoint, Vector2 endPoint)
    {
        #region ���_�̐U�蕪��

        var isClockWise1 = IsClockWise(startPoint, endPoint, victimMesh.vertices[index1]);
        var isClockWise2 = IsClockWise(startPoint, endPoint, victimMesh.vertices[index2]);
        var isClockWise3 = IsClockWise(startPoint, endPoint, victimMesh.vertices[index3]);

        var isDisconnected1 = victimMesh.disconnectedVertices.Contains(victimMesh.vertices[index1]);
        var isDisconnected2 = victimMesh.disconnectedVertices.Contains(victimMesh.vertices[index2]);
        var isDisconnected3 = victimMesh.disconnectedVertices.Contains(victimMesh.vertices[index3]);

        //�O�p�`�̂��ׂĂ̒��_������̑��ɂ���ꍇ�̓J�b�g�������ڒǉ�����
        if (isClockWise1 == isClockWise2 && isClockWise2 == isClockWise3)
        {
            if (isClockWise1)
            {
                cwSide.AddTriangle(index1, index2, index3, -1, -1, -1);
            }
            else
            {
                acwSide.AddTriangle(index1, index2, index3, -1, -1, -1);
            }

            return false;
        }

        Vector2[] cwExistingPoints = new Vector2[3];
        Vector2[] cwExistingUvs = new Vector2[3];
        Vector2[] cwNewPoints = new Vector2[2];
        Vector2[] cwNewUvs = new Vector2[2];

        Vector2[] acwExistingPoints = new Vector2[3];
        Vector2[] acwExistingUvs = new Vector2[3];
        Vector2[] acwNewPoints = new Vector2[2];
        Vector2[] acwNewUvs = new Vector2[2];


        bool triangleIsCwSide = false;
        bool IsTriangleOnCutEdge = false;

        //�J�b�g�̒[�ł��邱�Ƃ����o

        var isLineIntersectingCutter1_2 = HasIntersection(victimMesh.vertices[index1], victimMesh.vertices[index2], startPoint, endPoint);
        var isLineIntersectingCutter2_3 = HasIntersection(victimMesh.vertices[index2], victimMesh.vertices[index3], startPoint, endPoint);
        var isLineIntersectingCutter3_1 = HasIntersection(victimMesh.vertices[index3], victimMesh.vertices[index1], startPoint, endPoint);

        var IsLineIntersectingDisconnected1_2 = isDisconnected1 && isDisconnected2;
        var IsLineIntersectingDisconnected2_3 = isDisconnected2 && isDisconnected3;
        var IsLineIntersectingDisconnected3_1 = isDisconnected3 && isDisconnected1;


        var isCutOrderForward = true;

        // �O�p�`�̕ӂɑ΂���O�ς��v�Z���A������Ԃ��w���p�[�֐�
        static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        // �_���O�p�`�̓����ɂ��邩�𔻒肷�郁�\�b�h
        bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            // �e�ӂɑ΂��镄�����v�Z
            bool b1 = Sign(pt, v1, v2) < 0.0f;
            bool b2 = Sign(pt, v2, v3) < 0.0f;
            bool b3 = Sign(pt, v3, v1) < 0.0f;

            // �S�Ă̕�������v���Ă���ꍇ�A�_�͎O�p�`�̓����ɂ���
            return (b1 == b2) && (b2 == b3);
        }

        // UV���W����`��Ԃ���֐�
        Vector2 GetInterpolatedUV(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            // �o���Z���^�[���W���v�Z����
            float areaTotal = TriangleArea(p1, p2, p3);  // �O�p�`�S�̖̂ʐ�
            float area1 = TriangleArea(p, p2, p3);       // �_p�ƒ��_p2, p3�ō���镔���O�p�`�̖ʐ�
            float area2 = TriangleArea(p, p3, p1);       // �_p�ƒ��_p3, p1�ō���镔���O�p�`�̖ʐ�
            float area3 = TriangleArea(p, p1, p2);       // �_p�ƒ��_p1, p2�ō���镔���O�p�`�̖ʐ�

            // �o���Z���^�[���W�i�d�݁j���v�Z����
            float w1 = area1 / areaTotal;
            float w2 = area2 / areaTotal;
            float w3 = area3 / areaTotal;

            // �e���_��UV���W���o���Z���^�[���W�Ɋ�Â��ĕ��
            Vector2 interpolatedUV = w1 * uv1 + w2 * uv2 + w3 * uv3;

            return interpolatedUV;
        }

        // �O�p�`�̖ʐς��v�Z����֐��i2D���W�ŊO�ς𗘗p�j
        float TriangleArea(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return Mathf.Abs((p1.x * (p2.y - p3.y) + p2.x * (p3.y - p1.y) + p3.x * (p1.y - p2.y)) / 2.0f);
        }

        //�ؒf�ʂƌ������Ă��Ȃ��ꍇ�A�ؒf�̕K�v�͂Ȃ�
        if (!(isLineIntersectingCutter1_2 || isLineIntersectingCutter2_3 || isLineIntersectingCutter3_1))
        {
            cwSide.AddTriangle(index1, index2, index3, -1, -1, -1);
            return true;
        }

        //�ؒf�ʂƈ�񂾂��������Ă���ꍇ�A�ؒf�ʂ̒[�͎O�p�`�̒��ɂ���
        //���̏ꍇ4�̎O�p�`�ɕ�������
        //�����͂��Ă��Ȃ��̂őS�Ď��v��葤�Ƃ���
        else if ((isLineIntersectingCutter1_2 && !isLineIntersectingCutter2_3 && !isLineIntersectingCutter3_1) || (!isLineIntersectingCutter1_2 && isLineIntersectingCutter2_3 && !isLineIntersectingCutter3_1) || (!isLineIntersectingCutter1_2 && !isLineIntersectingCutter2_3 && isLineIntersectingCutter3_1))
        {
            IsTriangleOnCutEdge = true;
            if (isLineIntersectingCutter1_2)
            {
                GetIntersection(victimMesh.vertices[index1], victimMesh.vertices[index2], startPoint, endPoint, ref cwNewPoints[0]);
                cwNewUvs[0] = Vector2.Lerp(victimMesh.uv[index1], victimMesh.uv[index2], Vector2.Distance(victimMesh.vertices[index1], cwNewPoints[0]) / Vector2.Distance(victimMesh.vertices[index1], victimMesh.vertices[index2]));
                cwExistingPoints[0] = victimMesh.vertices[index1];
                cwExistingPoints[1] = victimMesh.vertices[index2];
                cwExistingPoints[2] = victimMesh.vertices[index3];

                cwExistingUvs[0] = victimMesh.uv[index1];
                cwExistingUvs[1] = victimMesh.uv[index2];
                cwExistingUvs[2] = victimMesh.uv[index3];
            }
            else if (isLineIntersectingCutter2_3)
            {
                GetIntersection(victimMesh.vertices[index2], victimMesh.vertices[index3], startPoint, endPoint, ref cwNewPoints[0]);
                cwNewUvs[0] = Vector2.Lerp(victimMesh.uv[index2], victimMesh.uv[index3], Vector2.Distance(victimMesh.vertices[index2], cwNewPoints[0]) / Vector2.Distance(victimMesh.vertices[index2], victimMesh.vertices[index3]));
                cwExistingPoints[0] = victimMesh.vertices[index2];
                cwExistingPoints[1] = victimMesh.vertices[index3];
                cwExistingPoints[2] = victimMesh.vertices[index1];

                cwExistingUvs[0] = victimMesh.uv[index2];
                cwExistingUvs[1] = victimMesh.uv[index3];
                cwExistingUvs[2] = victimMesh.uv[index1];
            }
            else
            {
                GetIntersection(victimMesh.vertices[index3], victimMesh.vertices[index1], startPoint, endPoint, ref cwNewPoints[0]);
                cwNewUvs[0] = Vector2.Lerp(victimMesh.uv[index3], victimMesh.uv[index1], Vector2.Distance(victimMesh.vertices[index3], cwNewPoints[0]) / Vector2.Distance(victimMesh.vertices[index3], victimMesh.vertices[index1]));
                cwExistingPoints[0] = victimMesh.vertices[index3];
                cwExistingPoints[1] = victimMesh.vertices[index1];
                cwExistingPoints[2] = victimMesh.vertices[index2];

                cwExistingUvs[0] = victimMesh.uv[index3];
                cwExistingUvs[1] = victimMesh.uv[index1];
                cwExistingUvs[2] = victimMesh.uv[index2];
            }

            isCutOrderForward = PointInTriangle(startPoint, victimMesh.vertices[index1], victimMesh.vertices[index2], victimMesh.vertices[index3]);
            cwNewPoints[1] = isCutOrderForward ? startPoint : endPoint;
            cwNewUvs[1] = GetInterpolatedUV(cwNewPoints[1], victimMesh.vertices[index1], victimMesh.vertices[index2], victimMesh.vertices[index3], victimMesh.uv[index1], victimMesh.uv[index2], victimMesh.uv[index3]);

        }

        //���_1�������Ⴄ���ɂ���ꍇ�A���_1���͎O�p�`�A���_2,3���͎l�p�`�ɂȂ�
        else if (isClockWise1 != isClockWise2 && isClockWise1 != isClockWise3)
        {
            if (isClockWise1)
            {
                triangleIsCwSide = true;

                cwExistingPoints[0] = victimMesh.vertices[index1];
                acwExistingPoints[0] = victimMesh.vertices[index2];
                acwExistingPoints[1] = victimMesh.vertices[index3];

                cwExistingUvs[0] = victimMesh.uv[index1];
                acwExistingUvs[0] = victimMesh.uv[index2];
                acwExistingUvs[1] = victimMesh.uv[index3];
            }
            else
            {
                triangleIsCwSide = false;

                acwExistingPoints[0] = victimMesh.vertices[index1];
                cwExistingPoints[0] = victimMesh.vertices[index2];
                cwExistingPoints[1] = victimMesh.vertices[index3];

                acwExistingUvs[0] = victimMesh.uv[index1];
                cwExistingUvs[0] = victimMesh.uv[index2];
                cwExistingUvs[1] = victimMesh.uv[index3];
            }


            //���_1�ƒ��_2�����Ԑ����A���_1�ƒ��_3�����Ԑ�����ɐV�����ؒf�_���ł���
            GetIntersection(victimMesh.vertices[index1], victimMesh.vertices[index2], startPoint, endPoint, ref cwNewPoints[0]);
            GetIntersection(victimMesh.vertices[index1], victimMesh.vertices[index3], startPoint, endPoint, ref cwNewPoints[1]);
            acwNewPoints[0] = cwNewPoints[0];
            acwNewPoints[1] = cwNewPoints[1];
            //UV��⊮����
            cwNewUvs[0] = Vector2.Lerp(victimMesh.uv[index1], victimMesh.uv[index2], Vector2.Distance(victimMesh.vertices[index1], cwNewPoints[0]) / Vector2.Distance(victimMesh.vertices[index1], victimMesh.vertices[index2]));
            cwNewUvs[1] = Vector2.Lerp(victimMesh.uv[index1], victimMesh.uv[index3], Vector2.Distance(victimMesh.vertices[index1], cwNewPoints[1]) / Vector2.Distance(victimMesh.vertices[index1], victimMesh.vertices[index3]));
            acwNewUvs[0] = cwNewUvs[0];
            acwNewUvs[1] = cwNewUvs[1];
        }
        //���_2�������Ⴄ���ɂ���ꍇ�A���_2���͎O�p�`�A���_3,1���͎l�p�`�ɂȂ�
        else if (isClockWise2 != isClockWise1 && isClockWise2 != isClockWise3)
        {
            //���l�ɏ�������
            if (isClockWise2)
            {
                triangleIsCwSide = true;
                cwExistingPoints[0] = victimMesh.vertices[index2];
                acwExistingPoints[0] = victimMesh.vertices[index3];
                acwExistingPoints[1] = victimMesh.vertices[index1];
                cwExistingUvs[0] = victimMesh.uv[index2];
                acwExistingUvs[0] = victimMesh.uv[index3];
                acwExistingUvs[1] = victimMesh.uv[index1];
            }
            else
            {
                triangleIsCwSide = false;
                acwExistingPoints[0] = victimMesh.vertices[index2];
                cwExistingPoints[0] = victimMesh.vertices[index3];
                cwExistingPoints[1] = victimMesh.vertices[index1];
                acwExistingUvs[0] = victimMesh.uv[index2];
                cwExistingUvs[0] = victimMesh.uv[index3];
                cwExistingUvs[1] = victimMesh.uv[index1];
            }

            GetIntersection(victimMesh.vertices[index2], victimMesh.vertices[index3], startPoint, endPoint, ref cwNewPoints[0]);
            GetIntersection(victimMesh.vertices[index2], victimMesh.vertices[index1], startPoint, endPoint, ref cwNewPoints[1]);
            acwNewPoints[0] = cwNewPoints[0];
            acwNewPoints[1] = cwNewPoints[1];
            cwNewUvs[0] = Vector2.Lerp(victimMesh.uv[index2], victimMesh.uv[index3], Vector2.Distance(victimMesh.vertices[index2], cwNewPoints[0]) / Vector2.Distance(victimMesh.vertices[index2], victimMesh.vertices[index3]));
            cwNewUvs[1] = Vector2.Lerp(victimMesh.uv[index2], victimMesh.uv[index1], Vector2.Distance(victimMesh.vertices[index2], cwNewPoints[1]) / Vector2.Distance(victimMesh.vertices[index2], victimMesh.vertices[index1]));
            acwNewUvs[0] = cwNewUvs[0];
            acwNewUvs[1] = cwNewUvs[1];
        }
        //���_3�������Ⴄ���ɂ���ꍇ�A���_3���͎O�p�`�A���_1,2���͎l�p�`�ɂȂ�
        else
        {
            //���l�ɏ�������
            if (isClockWise3)
            {
                triangleIsCwSide = true;
                cwExistingPoints[0] = victimMesh.vertices[index3];
                acwExistingPoints[0] = victimMesh.vertices[index1];
                acwExistingPoints[1] = victimMesh.vertices[index2];
                cwExistingUvs[0] = victimMesh.uv[index3];
                acwExistingUvs[0] = victimMesh.uv[index1];
                acwExistingUvs[1] = victimMesh.uv[index2];
            }
            else
            {
                triangleIsCwSide = false;
                acwExistingPoints[0] = victimMesh.vertices[index3];
                cwExistingPoints[0] = victimMesh.vertices[index1];
                cwExistingPoints[1] = victimMesh.vertices[index2];
                acwExistingUvs[0] = victimMesh.uv[index3];
                cwExistingUvs[0] = victimMesh.uv[index1];
                cwExistingUvs[1] = victimMesh.uv[index2];
            }

            GetIntersection(victimMesh.vertices[index3], victimMesh.vertices[index1], startPoint, endPoint, ref cwNewPoints[0]);
            GetIntersection(victimMesh.vertices[index3], victimMesh.vertices[index2], startPoint, endPoint, ref cwNewPoints[1]);
            acwNewPoints[0] = cwNewPoints[0];
            acwNewPoints[1] = cwNewPoints[1];
            cwNewUvs[0] = Vector2.Lerp(victimMesh.uv[index3], victimMesh.uv[index1], Vector2.Distance(victimMesh.vertices[index3], cwNewPoints[0]) / Vector2.Distance(victimMesh.vertices[index3], victimMesh.vertices[index1]));
            cwNewUvs[1] = Vector2.Lerp(victimMesh.uv[index3], victimMesh.uv[index2], Vector2.Distance(victimMesh.vertices[index3], cwNewPoints[1]) / Vector2.Distance(victimMesh.vertices[index3], victimMesh.vertices[index2]));
            acwNewUvs[0] = cwNewUvs[0];
            acwNewUvs[1] = cwNewUvs[1];
        }

        #endregion

        #region �O�p�`�����

        //�O�p�`�����
        if (IsTriangleOnCutEdge)
        {
            //cwSide.AddTriangle(cwExistingPoints[0], cwNewPoints[0], cwNewPoints[1], cwExistingUvs[0], cwNewUvs[0], cwNewUvs[1]);
            //cwSide.AddTriangle(cwExistingPoints[0], cwNewPoints[0], cwExistingPoints[1], cwExistingUvs[0], cwNewUvs[0], cwExistingUvs[1]);
            //cwSide.AddTriangle(cwExistingPoints[2], cwNewPoints[1], cwNewPoints[0], cwExistingUvs[2], cwNewUvs[1], cwNewUvs[0]);
            //cwSide.AddTriangle(cwExistingPoints[2], cwExistingPoints[0], cwNewPoints[1], cwExistingUvs[2], cwExistingUvs[0], cwNewUvs[1]);

            var isClockWiseExisting0 = isCutOrderForward ? IsClockWise(cwNewPoints[0], cwNewPoints[1], cwExistingPoints[0]) : IsClockWise(cwNewPoints[1], cwNewPoints[0], cwExistingPoints[0]);
            var isClockWiseExisting1 = isCutOrderForward ? IsClockWise(cwNewPoints[0], cwNewPoints[1], cwExistingPoints[1]) : IsClockWise(cwNewPoints[1], cwNewPoints[0], cwExistingPoints[1]);
            cwSide.AddTriangle(cwNewPoints[1], cwExistingPoints[0], cwExistingPoints[2], cwNewUvs[1], cwExistingUvs[0], cwExistingUvs[2], -1, -1, -1);
            cwSide.AddTriangle(cwNewPoints[1], cwExistingPoints[2], cwExistingPoints[1], cwNewUvs[1], cwExistingUvs[2], cwExistingUvs[1], -1, -1, -1);
            cwSide.AddTriangle(cwNewPoints[1], cwExistingPoints[1], cwNewPoints[0], cwNewUvs[1], cwExistingUvs[1], cwNewUvs[0], -1, -1, isClockWiseExisting1 ? 0 : 1);
            cwSide.AddTriangle(cwNewPoints[1], cwNewPoints[0], cwExistingPoints[0], cwNewUvs[1], cwNewUvs[0], cwExistingUvs[0], -1, isClockWiseExisting0 ? 0 : 1, -1);
            cwSide.disconnecteds.Add(cwNewPoints[0]);
            cwSide.disconnecteds.Add(cwNewPoints[1]);
            return true;
        }
        else if (triangleIsCwSide)
        {
            //���v��葤�͎O�p�`1��
            cwSide.AddTriangle(cwExistingPoints[0], cwNewPoints[1], cwNewPoints[0], cwExistingUvs[0], cwNewUvs[1], cwNewUvs[0], -1, 1, 1);
            cwSide.disconnecteds.Add(cwNewPoints[0]);
            cwSide.disconnecteds.Add(cwNewPoints[1]);

            //�����v��葤�͎O�p�`2��
            acwSide.AddTriangle(acwExistingPoints[1], acwExistingPoints[0], acwNewPoints[0], acwExistingUvs[1], acwExistingUvs[0], acwNewUvs[0], -1, -1, 0);
            acwSide.AddTriangle(acwExistingPoints[1], acwNewPoints[0], acwNewPoints[1], acwExistingUvs[1], acwNewUvs[0], acwNewUvs[1], -1, 0, 0);
            acwSide.disconnecteds.Add(acwNewPoints[0]);
            acwSide.disconnecteds.Add(acwNewPoints[1]);
        }
        else
        {
            //�����v��葤�͎O�p�`1��
            acwSide.AddTriangle(acwExistingPoints[0], acwNewPoints[1], acwNewPoints[0], acwExistingUvs[0], acwNewUvs[1], acwNewUvs[0], -1, 0, 0);
            acwSide.disconnecteds.Add(acwNewPoints[0]);
            acwSide.disconnecteds.Add(acwNewPoints[1]);

            //���v��葤�͎O�p�`2��
            cwSide.AddTriangle(cwExistingPoints[1], cwExistingPoints[0], cwNewPoints[0], cwExistingUvs[1], cwExistingUvs[0], cwNewUvs[0], -1, -1, 1);
            cwSide.AddTriangle(cwExistingPoints[1], cwNewPoints[0], cwNewPoints[1], cwExistingUvs[1], cwNewUvs[0], cwNewUvs[1], -1, 1, 1);
            cwSide.disconnecteds.Add(cwNewPoints[0]);
            cwSide.disconnecteds.Add(cwNewPoints[1]);
        }

        #endregion

        return false;
    }

    public static bool IsClockWise(Vector2 point1, Vector2 point2, Vector2 testPoint)
    {
        return (point2.x - point1.x) * (testPoint.y - point2.y) - (point2.y - point1.y) * (testPoint.x - point2.x) < 0;
    }

    public static bool GetIntersection(Vector2 line1_start, Vector2 line1_end, Vector2 line2_start, Vector2 line2_end, ref Vector2 intersection)
    {
        float S1 = (line2_end.x - line2_start.x) * (line1_start.y - line2_start.y) - (line2_end.y - line2_start.y) * (line1_start.x - line2_start.x);
        float S2 = (line2_end.x - line2_start.x) * (line2_start.y - line1_end.y) - (line2_end.y - line2_start.y) * (line2_start.x - line1_end.x);
        if (S1 + S2 == 0f) return false;

        var x = line1_start.x + (line1_end.x - line1_start.x) * S1 / (S1 + S2);
        var y = line1_start.y + (line1_end.y - line1_start.y) * S1 / (S1 + S2);
        intersection = new Vector2(x, y);
        return true;
    }
    //public static bool HasIntersection(Vector2 line_start, Vector2 line_end, Vector2 lineStrip_start, Vector2 lineStrip_end)
    //{
    //    var a = (line_start.x - line_end.x) * (lineStrip_start.y - line_start.y) + (line_start.y - line_end.y) * (line_start.x - lineStrip_start.x);
    //    var b = (line_start.x - line_end.x) * (lineStrip_end.y - line_start.y) + (line_start.y - line_end.y) * (line_start.x - lineStrip_end.x);
    //    return a * b < 0;
    //}

    public static bool HasIntersection(
    Vector2 line1_start,
    Vector2 line2_start,
    Vector2 line3_start,
    Vector2 line4_start
    )
    {

        var d = (line2_start.x - line1_start.x) * (line4_start.y - line3_start.y) - (line2_start.y - line1_start.y) * (line4_start.x - line3_start.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((line3_start.x - line1_start.x) * (line4_start.y - line3_start.y) - (line3_start.y - line1_start.y) * (line4_start.x - line3_start.x)) / d;
        var v = ((line3_start.x - line1_start.x) * (line2_start.y - line1_start.y) - (line3_start.y - line1_start.y) * (line2_start.x - line1_start.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        return true;
    }

    public static float BitDecrement(float x)
    {
        int bits = BitConverter.SingleToInt32Bits(x);

        if ((bits & 0x7F800000) >= 0x7F800000)
        {
            // NaN returns NaN
            // -Infinity returns -Infinity
            // +Infinity returns float.MaxValue
            return (bits == 0x7F800000) ? float.MaxValue : x;
        }

        if (bits == 0x00000000)
        {
            // +0.0 returns -float.Epsilon
            return -float.Epsilon;
        }

        // Negative values need to be incremented
        // Positive values need to be decremented

        bits += ((bits < 0) ? +1 : -1);
        return BitConverter.Int32BitsToSingle(bits);
    }

    public static float BitIncrement(float x)
    {
        int bits = BitConverter.SingleToInt32Bits(x);

        if ((bits & 0x7F800000) >= 0x7F800000)
        {
            // NaN returns NaN
            // -Infinity returns float.MinValue
            // +Infinity returns +Infinity
            return (bits == unchecked((int)(0xFF800000))) ? float.MinValue : x;
        }

        if (bits == unchecked((int)(0x80000000)))
        {
            // -0.0 returns float.Epsilon
            return float.Epsilon;
        }

        // Negative values need to be decremented
        // Positive values need to be incremented

        bits += ((bits < 0) ? -1 : +1);
        return BitConverter.Int32BitsToSingle(bits);
    }

}

