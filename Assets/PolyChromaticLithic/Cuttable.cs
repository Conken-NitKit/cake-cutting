using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mesh2DAssigner))]
[DisallowMultipleComponent]
public class Cuttable : MonoBehaviour
{
    private void Awake()
    {
        Cutter.cuttables.Add(this);
    }

    public void Cut(Vector2 startPoint, Vector2 endPoint)
    {
        startPoint = (Vector2)this.transform.InverseTransformPoint(startPoint);
        endPoint = (Vector2)this.transform.InverseTransformPoint(endPoint);
        var cutMeshes = PolygonCutter.SegmentCut(this.GetComponent<Mesh2DAssigner>().Mesh2D, startPoint, endPoint);
        foreach (var cutMesh in cutMeshes)
        {
            GameObject go = new GameObject("Cake",typeof(MeshFilter), typeof(MeshRenderer), typeof(Mesh2DAssigner), typeof(Cuttable));
            go.transform.position = this.transform.position;
            go.transform.rotation = this.transform.rotation;
            go.transform.localScale = this.transform.localScale;
            go.GetComponent<Mesh2DAssigner>().Mesh2D = cutMesh;
            go.GetComponent<MeshRenderer>().material = this.GetComponent<MeshRenderer>().material;
            //ColliderCreator.SetCollider(cutMesh.ToMesh(), go.GetComponent<PolygonCollider2D>());
        }
        Destroy(gameObject);

    }    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
