using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
[DisallowMultipleComponent]
public class Mesh2DAssigner : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private PolygonCollider2D polygonCollider;
    
    
    private Mesh2D mesh2d;
    public Mesh2D Mesh2D
    {
        get
        {
            if (mesh2d == null)
            {
                mesh2d = new Mesh2D();
            }
            return mesh2d;
        }
        set
        {
            mesh2d = value;
            meshFilter.mesh = mesh2d.ToMesh();
            //ColliderCreator.SetCollider(mesh2d.ToMesh(), polygonCollider);
        }
    }
    
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
