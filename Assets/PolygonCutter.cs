using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PolygonCutter
{
    private class MeshCutSide
    {
        private List<Vector2> vertices = new();
        private List<int> triangles = new();
        private List<Vector2> uvs = new();

        public void ClearAll()
        {
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
        }

        //元の頂点を参照して三角形を追加
        public void AddTriangle(int index1, int index2, int index3, bool forceClockWise = true)
        {

            Debug.Log($"Addtriangle_1: {index1}, {index2}, {index3}");
            var nowIndex = vertices.Count;
            if (forceClockWise)
            {
                if (!IsClockWise(victimMesh.vertices[index1], victimMesh.vertices[index2], victimMesh.vertices[index3]))
                {
                    (index3, index2) = (index2, index3);

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
            }
            else
            {
                triangles.Add(nowIndex + 2);
                triangles.Add(nowIndex + 1);
                triangles.Add(nowIndex + 0);
            }

            vertices.Add(victimMesh.vertices[index1]);
            vertices.Add(victimMesh.vertices[index2]);
            vertices.Add(victimMesh.vertices[index3]);


            uvs.Add(victimMesh.uv[index1]);
            uvs.Add(victimMesh.uv[index2]);
            uvs.Add(victimMesh.uv[index3]);

        }

        public void AddTriangle(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 uv1, Vector2 uv2, Vector2 uv3, bool forceClockWise = true)
        {
            Debug.Log($"Addtriangle_2: {point1}, {point2}, {point3}");
            
            var nowIndex = vertices.Count;
            if (forceClockWise)
            {
                if (!IsClockWise(point1, point2, point3))
                {
                    (point3, point2) = (point2, point3);
                    (uv3, uv2) = (uv2, uv3);

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
            }
            else
            {

                triangles.Add(nowIndex + 2);
                triangles.Add(nowIndex + 1);
                triangles.Add(nowIndex + 0);

            }


            vertices.Add(point1);
            vertices.Add(point2);
            vertices.Add(point3);

            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);

        }

        public Mesh2D ToMesh2D()
        {
            var mesh = new Mesh2D();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            return mesh;
        }

    }



    private static Mesh2D victimMesh;
    private static MeshCutSide cwSide;
    private static MeshCutSide acwSide;

    public static Mesh2D[] Cut(Mesh2D victim, Vector2 startPoint, Vector2 endPoint)
    {
        victimMesh = victim;

        //切断する直線に対し時計回り(clockwise)側と反時計回り(anti-clockwise)側に分ける
        cwSide = new MeshCutSide();
        acwSide = new MeshCutSide();


        //三角形を振り分ける
        for (int triangleCount = 0; triangleCount < victim.triangles.Count; triangleCount += 3)
        {
            var index1 = victim.triangles[triangleCount];
            var index2 = victim.triangles[triangleCount + 1];
            var index3 = victim.triangles[triangleCount + 2];

            CutTriangle(index1, index2, index3, startPoint, endPoint);

        }

        var cwMesh = cwSide.ToMesh2D();
        var acwMesh = acwSide.ToMesh2D();

        return new Mesh2D[] { cwMesh, acwMesh };

    }

    public static Mesh2D[] SegmentCut(Mesh2D victim, Vector2 startPoint, Vector2 endPoint)
    {
        victimMesh = victim;
        //切断する直線に対し時計回り(clockwise)側と反時計回り(anti-clockwise)側に分ける
        cwSide = new MeshCutSide();
        acwSide = new MeshCutSide();
        //三角形を振り分ける
        for (int triangleCount = 0; triangleCount < victim.triangles.Count; triangleCount += 3)
        {
            var index1 = victim.triangles[triangleCount];
            var index2 = victim.triangles[triangleCount + 1];
            var index3 = victim.triangles[triangleCount + 2];
            //SegmentCutTriangle(index1, index2, index3, startPoint, endPoint);
        }
        var cwMesh = cwSide.ToMesh2D();
        var acwMesh = acwSide.ToMesh2D();
        return new Mesh2D[] { cwMesh, acwMesh };
    }

    private static void CutTriangle(int index1, int index2, int index3, Vector2 startPoint, Vector2 endPoint)
    {
        var isClockWise1 = IsClockWise(startPoint, endPoint, victimMesh.vertices[index1]);
        var isClockWise2 = IsClockWise(startPoint, endPoint, victimMesh.vertices[index2]);
        var isClockWise3 = IsClockWise(startPoint, endPoint, victimMesh.vertices[index3]);

        //三角形のすべての頂点が一方の側にある場合はカットせず直接追加する
        if (isClockWise1 == isClockWise2 && isClockWise2 == isClockWise3)
        {
            if (isClockWise1)
            {
                cwSide.AddTriangle(index1, index2, index3);
            }
            else
            {
                acwSide.AddTriangle(index1, index2, index3);
            }

            return;
        }

        Vector2[] cwExistingPoints = new Vector2[2];
        Vector2[] cwExistingUvs = new Vector2[2];
        Vector2[] cwNewPoints = new Vector2[2];
        Vector2[] cwNewUvs = new Vector2[2];

        Vector2[] acwExistingPoints = new Vector2[2];
        Vector2[] acwExistingUvs = new Vector2[2];
        Vector2[] acwNewPoints = new Vector2[2];
        Vector2[] acwNewUvs = new Vector2[2];


        bool triangleIsCwSide;
        //頂点1だけが違う側にいる場合、頂点1側は三角形、頂点2,3側は四角形になる
        if (isClockWise1 != isClockWise2 && isClockWise1 != isClockWise3)
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


            //頂点1と頂点2を結ぶ線分、頂点1と頂点3を結ぶ線分上に新しい切断点ができる
            GetIntersection(victimMesh.vertices[index1], victimMesh.vertices[index2], startPoint, endPoint, ref cwNewPoints[0]);
            GetIntersection(victimMesh.vertices[index1], victimMesh.vertices[index3], startPoint, endPoint, ref cwNewPoints[1]);
            acwNewPoints[0] = cwNewPoints[0];
            acwNewPoints[1] = cwNewPoints[1];

            //UVを補完する
            cwNewUvs[0] = Vector2.Lerp(victimMesh.uv[index1], victimMesh.uv[index2], Vector2.Distance(victimMesh.vertices[index1], cwNewPoints[0]) / Vector2.Distance(victimMesh.vertices[index1], victimMesh.vertices[index2]));
            cwNewUvs[1] = Vector2.Lerp(victimMesh.uv[index1], victimMesh.uv[index3], Vector2.Distance(victimMesh.vertices[index1], cwNewPoints[1]) / Vector2.Distance(victimMesh.vertices[index1], victimMesh.vertices[index3]));
            acwNewUvs[0] = cwNewUvs[0];
            acwNewUvs[1] = cwNewUvs[1];
        }
        //頂点2だけが違う側にいる場合、頂点2側は三角形、頂点3,1側は四角形になる
        else if (isClockWise2 != isClockWise1 && isClockWise2 != isClockWise3)
        {
            //同様に処理する
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
        //頂点3だけが違う側にいる場合、頂点3側は三角形、頂点1,2側は四角形になる
        else
        {
            //同様に処理する
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

        //全部出力
        Debug.Log("cwExistingPoints[0] = " + cwExistingPoints[0]);
        Debug.Log("cwExistingPoints[1] = " + cwExistingPoints[1]);
        Debug.Log("acwExistingPoints[0] = " + acwExistingPoints[0]);
        Debug.Log("acwExistingPoints[1] = " + acwExistingPoints[1]);
        Debug.Log("cwNewPoints[0] = " + cwNewPoints[0]);
        Debug.Log("cwNewPoints[1] = " + cwNewPoints[1]);
        Debug.Log("acwNewPoints[0] = " + acwNewPoints[0]);
        Debug.Log("acwNewPoints[1] = " + acwNewPoints[1]);
        Debug.Log("triangleIsCwSide = " + triangleIsCwSide);


        //三角形を作る
        if (triangleIsCwSide)
        {
            //時計回り側は三角形1個
            cwSide.AddTriangle(cwExistingPoints[0], cwNewPoints[0], cwNewPoints[1], cwExistingUvs[0], cwNewUvs[0], cwNewUvs[1]);

            //反時計回り側は三角形2個
            acwSide.AddTriangle(acwExistingPoints[0], acwExistingPoints[1], acwNewPoints[0], acwExistingUvs[0], acwExistingUvs[1], acwNewUvs[0]);
            acwSide.AddTriangle(acwExistingPoints[1], acwNewPoints[1], acwNewPoints[0], acwExistingUvs[1], acwNewUvs[1], acwNewUvs[0]);
        }
        else
        {
            //反時計回り側は三角形1個
            acwSide.AddTriangle(acwExistingPoints[0], acwNewPoints[0], acwNewPoints[1], acwExistingUvs[0], acwNewUvs[0], acwNewUvs[1]);

            //時計回り側は三角形2個
            cwSide.AddTriangle(cwExistingPoints[0], cwExistingPoints[1], cwNewPoints[0], cwExistingUvs[0], cwExistingUvs[1], cwNewUvs[0]);
            cwSide.AddTriangle(cwExistingPoints[1], cwNewPoints[1], cwNewPoints[0], cwExistingUvs[1], cwNewUvs[1], cwNewUvs[0]);
        }

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

    public static bool HasIntersection(Vector2 line_start, Vector2 line_end, Vector2 lineStrip_start, Vector2 lineStrip_end)
    {
        var a = (line_start.x - line_end.x) * (lineStrip_start.y - line_start.y) + (line_start.y - line_end.y) * (line_start.x - lineStrip_start.x);
        var b = (line_start.x - line_end.x) * (lineStrip_end.y - line_start.y) + (line_start.y - line_end.y) * (line_start.x - lineStrip_end.x);
        return a * b < 0;
    }


}

