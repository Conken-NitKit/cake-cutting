using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
    private static GameObject cuttingBoardGameObject;
    public static GameObject CuttingBoardGameObject
    {
        get
        {
            if (cuttingBoardGameObject == null)
            {
                cuttingBoardGameObject = GameObject.Find("CuttingBoard");
            }
            return cuttingBoardGameObject;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
