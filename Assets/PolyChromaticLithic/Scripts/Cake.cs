using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{
    private static List<Cake> allCake;
    public static List<Cake> AllCake
    {
        get
        {
            if (allCake == null) allCake = new List<Cake>();
            return allCake;
        }
        private set => allCake = value;
    }

    public static float AllCakeMass
    {
        get
        {
            float mass = 0;
            foreach (var cake in AllCake)
            {
                mass += cake.Mass;
            }
            return mass;
        }
    }


    private Mesh2DAssigner mesh2DAssigner;

    public Vector3 center;

    private float? mass;

    public float Mass
    {
        get 
        { 
            if (mass == null) mass = GetMass();
            return (float)mass;
        }
    }




    public static void AddCake(Cake cake)
    {
        if (AllCake == null) AllCake = new List<Cake>();
        AllCake.Add(cake);
        if (AllCake.Count >= CuttingFlowManager.Instance.TargetCuttingCount)
        {
            CuttingFlowManager.Instance.IsCakeOverTargetSlices = true;
        }
    }

    public float GetMass()
    {
        return mesh2DAssigner.Mesh2D.CalcurateArea();
    }

    private void Awake()
    {
        mesh2DAssigner = GetComponent<Mesh2DAssigner>();
    }

    private void Start()
    {
        GetComponent<MeshRenderer>().material.color = NumberToColor(Mass);
    }

    public static bool IsDraggingAnyCake()
    {
        foreach (var cake in AllCake)
        {
            if (cake.isDragging) return true;
        }
        return false;
    }

    private Vector3 offset;
    private bool isDragging = false;

    // Update is called once per frame
    private void Update()
    {
        if (!CuttingFlowManager.Instance.ArrowDrag)
        {
            isDragging = false;
            return;
        }
        if (Input.GetMouseButtonUp(0)) isDragging = false;
        if (isDragging)
        {
            Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10);
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;
            transform.position = currentPosition;
        }
        //マウス右ボタンでドラッグして移動できるようにする
        //RayCastでクリックしているか確認する
        else if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
                }
            }
        }
    }

    private Color NumberToColor(float id)
    {
        return Color.HSVToRGB(id * 123f % 0.1f * 10f, 0.3f, 0.95f);
    }

}
