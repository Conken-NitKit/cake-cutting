using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    
    void Start()
    {
        transform.position = new Vector3(25, 0, 0);
        CuttingBoard.CuttingBoardGameObject.transform.position = new Vector3(0, -3.2f, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OpenTable();
        }
    }

    public void OpenTable()
    {
        transform.DOMoveX(10, 0.2f);
        CuttingBoard.CuttingBoardGameObject.transform.DOMoveX(-7, 0.2f);
    }
}
