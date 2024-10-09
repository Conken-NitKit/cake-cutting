using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public float size = 8f;
    [SerializeField] private TextMeshProUGUI text;

    private HashSet<Cake> cakes = new HashSet<Cake>();

    private float mass = 0;
    private float massTmp = 0;


    private void Awake()
    {
        text.text = "0%";
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
                mass += cake.GetMass();
            }
        }
        if (mass != massTmp)
        {
            text.text = (mass / Cake.AllCakeMass * 100).ToString() + "%";
        }
        massTmp = mass;
        
    }
}
