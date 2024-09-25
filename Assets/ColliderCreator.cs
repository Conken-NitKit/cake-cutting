////https://www.h3xed.com/programming/automatically-create-polygon-collider-2d-from-2d-mesh-in-unity

//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class ColliderCreator
//{
//    static public void SetCollider(Mesh mesh, PolygonCollider2D polygonCollider)
//    {

//        // Get triangles and vertices from mesh
//        int[] triangles = mesh.triangles;
//        Vector3[] vertices = mesh.vertices;

//        // Get just the outer edges from the mesh's triangles (ignore or remove any shared edges)
//        Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
//        for (int i = 0; i < triangles.Length; i += 3)
//        {
//            for (int e = 0; e < 3; e++)
//            {
//                int vert1 = triangles[i + e];
//                int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
//                string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
//                if (edges.ContainsKey(edge))
//                {
//                    edges.Remove(edge);
//                }
//                else
//                {
//                    edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
//                }
//            }
//        }

//        // Create edge lookup (Key is first vertex, Value is second vertex, of each edge)
//        Dictionary<int, int> lookup = new Dictionary<int, int>();
//        foreach (KeyValuePair<int, int> edge in edges.Values)
//        {
//            if (lookup.ContainsKey(edge.Key) == false)
//            {
//                lookup.Add(edge.Key, edge.Value);
//            }
//        }

//        if (lookup.Count == 0) return;

//        polygonCollider.pathCount = 0;

//        // Loop through edge vertices in order
//        int startVert = lookup.Select(x => x.Key).OrderBy(x => x).First();
//        int nextVert = startVert;
//        int highestVert = startVert;
//        List<Vector2> colliderPath = new List<Vector2>();
//        while (true)
//        {

//            Debug.Log(nextVert);
//            // Add vertex to collider path
//            colliderPath.Add(vertices[nextVert]);

//            // Get next vertex
//            nextVert = lookup[nextVert];

//            // Store highest vertex (to know what shape to move to next)
//            if (nextVert > highestVert)
//            {
//                highestVert = nextVert;
//            }

//            // Shape complete
//            if (nextVert == startVert)
//            {

//                // Add path to polygon collider
//                polygonCollider.pathCount++;
//                polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
//                colliderPath.Clear();

//                // Go to next shape if one exists
//                if (lookup.ContainsKey(highestVert + 1))
//                {

//                    // Set starting and next vertices
//                    startVert = highestVert + 1;
//                    nextVert = startVert;

//                    // Continue to next loop
//                    continue;
//                }

//                // No more verts
//                break;
//            }
//        }
//    }
//}
