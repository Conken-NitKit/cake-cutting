using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mesh2DAssigner))]
[DisallowMultipleComponent]
public class Cuttable : MonoBehaviour
{
    public float minSize = 0.001f;

    private void Awake()
    {
        Cutter.cuttables.Add(this);
    }

    public void Cut(Vector2 startPoint, Vector2 endPoint)
    {
        Cake.AllCake.Remove(this.gameObject.GetComponent<Cake>());
        startPoint = (Vector2)this.transform.InverseTransformPoint(startPoint);
        endPoint = (Vector2)this.transform.InverseTransformPoint(endPoint);
        var cutMeshes = PolygonCutter.SegmentCut(this.GetComponent<Mesh2DAssigner>().Mesh2D, startPoint, endPoint);
        foreach (var cutMesh in cutMeshes)
        {
            if (cutMesh.CalcurateArea() < minSize) continue;
            GameObject go = new GameObject("Cake", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(Mesh2DAssigner), typeof(Cake), typeof(Cuttable));
            go.transform.position = this.transform.position;
            go.transform.rotation = this.transform.rotation;
            go.transform.localScale = this.transform.localScale;
            go.GetComponent<Mesh2DAssigner>().Mesh2D = cutMesh;
            var mr = go.GetComponent<MeshRenderer>();
            mr.material = this.GetComponent<MeshRenderer>().material;
            go.GetComponent<Cake>().center = cutMesh.CalcurateCentroid();
            go.transform.parent = CuttingBoard.CuttingBoardGameObject.transform;
            Cake.AddCake(go.GetComponent<Cake>());
            //ColliderCreator.SetCollider(cutMesh.ToMesh(), go.GetComponent<PolygonCollider2D>());
        }
        Destroy(gameObject);

    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{

        //}
        //else if (Input.GetKeyDown(KeyCode.A))
        // {
        //     var cutMeshes = PolygonCutter.Cut(this.GetComponent<Mesh2DAssigner>().Mesh2D, new Vector2(0, -0.5f), new Vector2(1, 0.5f));
        //     foreach (var cutMesh in cutMeshes)
        //     {
        //         var separateMesh = MeshSeparator.SeparateMesh(cutMesh);
        //         foreach (var mesh in separateMesh)
        //         {
        //             GameObject go = new GameObject("name", typeof(MeshFilter), typeof(MeshRenderer), typeof(Mesh2DAssigner), typeof(Cuttable));
        //             go.GetComponent<Mesh2DAssigner>().Mesh2D = mesh;
        //         }
        //     }
        //     Destroy(gameObject);
        // }
    }

}
