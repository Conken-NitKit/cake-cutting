using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[DisallowMultipleComponent]
public class Mesh2DAssigner : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    
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
            meshCollider.sharedMesh = meshFilter.mesh;
            Debug.Log($"{mesh2d.vertices.Count} vertices, {mesh2d.triangles.Count / 3} triangles");
            Debug.Log($"Size: {mesh2d.CalcurateArea()}");
        }
    }
    
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }
}
