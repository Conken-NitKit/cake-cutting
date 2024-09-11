using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mesh2DAssigner))]
[DisallowMultipleComponent]
public class Cuttable : MonoBehaviour
{
    
    
    public void Cut(Vector2 startPoint, Vector2 endPoint)
    {
        var cutMeshes = PolygonCutter.Cut(this.GetComponent<Mesh2DAssigner>().Mesh2D, startPoint, endPoint);
        foreach (var cutMesh in cutMeshes)
        {
            GameObject go = new GameObject("name",typeof(PolygonCollider2D) ,typeof(MeshFilter), typeof(MeshRenderer), typeof(Mesh2DAssigner), typeof(Cuttable));
            go.GetComponent<Mesh2DAssigner>().Mesh2D = cutMesh;
            ColliderCreator.SetCollider(cutMesh.ToMesh(), go.GetComponent<PolygonCollider2D>());
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
       if (Input.GetKeyDown(KeyCode.Space))
       {
            
       }
       else if (Input.GetKeyDown(KeyCode.A))
        {
            var cutMeshes = PolygonCutter.Cut(this.GetComponent<Mesh2DAssigner>().Mesh2D, new Vector2(0, -0.5f), new Vector2(1, 0.5f));
            foreach (var cutMesh in cutMeshes)
            {
                var separateMesh = MeshSeparator.SeparateMesh(cutMesh);
                foreach (var mesh in separateMesh)
                {
                    GameObject go = new GameObject("name", typeof(MeshFilter), typeof(MeshRenderer), typeof(Mesh2DAssigner), typeof(Cuttable));
                    go.GetComponent<Mesh2DAssigner>().Mesh2D = mesh;
                }
            }
            Destroy(gameObject);
        }
    }
}
