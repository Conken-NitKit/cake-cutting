using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mesh2DAssigner))]
public class ImportMesh : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        
        GetComponent<Mesh2DAssigner>().Mesh2D = Mesh2D.ToMesh2D(mesh).NormalizeSize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
