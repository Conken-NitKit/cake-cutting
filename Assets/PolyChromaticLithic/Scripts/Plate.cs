using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private static List<Plate> plates = new List<Plate>();

    public static float[] GetSizes()
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
        mass = 0;
        foreach (var cake in Cake.AllCake)
        {
            if ((cake.transform.position + cake.center - transform.position).sqrMagnitude <= Mathf.Pow(size, 2))
            {
                mass += cake.Mass;
                if (cakes.Contains(cake)) continue;
                cakes.Add(cake);
                cake.transform.parent = transform;
                CheckCakeServedOnAllPlates();
            }
            else
            {
                if (cakes.Contains(cake))
                {
                    cake.transform.parent = cuttingBoard.transform;
                    cakes.Remove(cake);
                    CheckCakeServedOnAllPlates();
                }
            }
        }
        if (mass != massTmp)
        {
            text.text = (mass / Cake.AllCakeMass * 100).ToString() + "%";
        }
        massTmp = mass;
    }

    private static void CheckCakeServedOnAllPlates()
    {
        Debug.Log(string.Join(',', plates.Select(x => x.cakes.Count)));
        foreach (var plate in plates)
        {
            if (plate.cakes.Count == 0)
            {
                CuttingFlowManager.Instance.IsCakeServedOnAllPlates = false;
                return;
            }
        }
        CuttingFlowManager.Instance.IsCakeServedOnAllPlates = true;
    }
}
