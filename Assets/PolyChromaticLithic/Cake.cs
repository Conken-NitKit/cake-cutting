using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{
    static private List<Cake> allCake;
    static public List<Cake> AllCake
    {
        get
        {
            if (allCake == null) allCake = new List<Cake>();
            return allCake;
        }
        private set => allCake = value;
    }

    static public float AllCakeMass
    {
        get
        {
            float mass = 0;
            foreach (var cake in AllCake)
            {
                mass += cake.GetMass();
            }
            return mass;
        }
    }

    private Mesh2DAssigner mesh2DAssigner;

    public Vector3 center;
    
    public static void AddCake(Cake cake)
    {   
        if (AllCake == null) AllCake = new List<Cake>();
        AllCake.Add(cake);
    }

    public float GetMass()
    {
        return mesh2DAssigner.Mesh2D.CalcurateArea();
    }
    
    private void Awake()
    {
        mesh2DAssigner = GetComponent<Mesh2DAssigner>();
    }

    private Vector3 offset;
    private bool isDrugging = false;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonUp(1)) isDrugging = false;
        if (isDrugging)
        {
            Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10);
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;
            transform.position = currentPosition;
        }
        //マウス右ボタンでドラッグして移動できるようにする
        //RayCastでクリックしているか確認する
        else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isDrugging = true;
                    offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
                }
            }
        }
    }
}
