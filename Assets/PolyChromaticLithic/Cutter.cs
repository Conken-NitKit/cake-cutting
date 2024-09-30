using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour
{
    [SerializeField] private GameObject StartPoint;
    [SerializeField] private GameObject EndPoint;
    [SerializeField] private LineRenderer LineRenderer;

    public static List<Cuttable> cuttables = new();


    // Start is called before the first frame update
    private void Start()
    {

    }

    private bool isDrugging = false;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrugging = true;
            StartPoint.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            StartPoint.transform.position = new Vector3(StartPoint.transform.position.x, StartPoint.transform.position.y, 0);
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDrugging = false;

            Vector3 pointA = StartPoint.transform.position;
            Vector3 pointB = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pointB = new Vector3(pointB.x, pointB.y, 0);
            EndPoint.transform.position = pointB;



            //Ray2D ray = new Ray2D(pointA, pointB - pointA);
            //float distance = Vector3.Distance(pointA, pointB);

            //RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, distance);

            //foreach (RaycastHit2D hit in hits)
            //{
            //    hit.collider.gameObject.TryGetComponent(out Cuttable cuttable);
            //    if (cuttable == null) continue;
            //    cuttable.Cut(pointA, pointB);
            //}

            //Staticにしたせいで列挙中にも変更されるからコピーを列挙する
            var cuttables_copy = new List<Cuttable>(cuttables);

            //カットのたびに新しく生成されるので削除しておく
            cuttables.Clear();

            foreach (var cuttable in cuttables_copy)
            {
                cuttable.Cut(pointA, pointB);
            }

            
        }

        if (isDrugging)
        {
            LineRenderer.positionCount = 2;
            LineRenderer.SetPosition(0, StartPoint.transform.position);
            LineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
