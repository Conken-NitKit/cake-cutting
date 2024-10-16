using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public float size = 8f;
    [SerializeField] private TextMeshProUGUI text;
    private GameObject cuttingBoard;

    private HashSet<Cake> cakes = new HashSet<Cake>();

    private float mass = 0;
    private float massTmp = 0;

    static private List<Plate> plates = new List<Plate>();

    static public float[] GetSizes()
    {
        float[] sizes = new float[plates.Count];
        for (int i = 0; i < plates.Count; i++)
        {
            sizes[i] = plates[i].massTmp;
        }
        return sizes;
    }




    private void Awake()
    {
        text.text = "0%";
        plates.Add(this);
    }

    private void Start()
    {
        cuttingBoard = GameObject.Find("CuttingBoard");
    }

    // Update is called once per frame
    private void Update()
    {
        cakes.Clear();
        mass = 0;
        foreach (var cake in Cake.AllCake)
        {
            if ((cake.transform.position + cake.center - transform.position).sqrMagnitude <= Mathf.Pow(size, 2))
            {
                cakes.Add(cake);
                cake.transform.parent = transform;
                mass += cake.Mass;
            }
            else
            {
                if (cake.transform.parent == transform)
                {
                    cake.transform.parent = cuttingBoard.transform;
                }
            }
        }
        if (mass != massTmp)
        {
            text.text = (mass / Cake.AllCakeMass * 100).ToString() + "%";
        }
        massTmp = mass;
        
    }
}
