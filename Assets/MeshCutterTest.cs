using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCutterTest : MonoBehaviour
{
    public Transform Line1_start;
    public Transform Line1_end;
    public Transform Line2_start;
    public Transform Line2_end;
    public Transform Intersection1;
    public Transform Intersection2;

    public LineRenderer Line1;
    public LineRenderer Line2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Line1.SetPosition(0, Line1_start.position);
        Line1.SetPosition(1, Line1_end.position);
        Line2.SetPosition(0, Line2_start.position);
        Line2.SetPosition(1, Line2_end.position);
        
        Line2_start.GetComponent<SpriteRenderer>().color = PolygonCutter.IsClockWise(Line1_start.position, Line1_end.position, Line2_start.position) ? Color.red : Color.blue;
        Line2_end.GetComponent<SpriteRenderer>().color = PolygonCutter.IsClockWise(Line1_start.position, Line1_end.position, Line2_end.position) ? Color.red : Color.blue;

        Vector2 Intersection1_pos = new();
        PolygonCutter.GetIntersection(Line1_start.position, Line1_end.position, Line2_start.position, Line2_end.position, ref Intersection1_pos);
        Intersection1.position = Intersection1_pos;

        if (PolygonCutter.HasIntersection(Line1_start.position, Line1_end.position, Line2_start.position, Line2_end.position))
        {
            Intersection2.position = Intersection1_pos;
            Intersection2.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            Intersection2.position = new Vector2(1000, 1000);
            Intersection2.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
