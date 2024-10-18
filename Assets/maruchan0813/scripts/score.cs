using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt;
    private ResultData result;
    // Start is called before the first frame update
    void Start()
    {
        txt.text = "100";
        result = ResultDataHandler.Instance.result;
        txt.text = calcscore(result).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int calcscore(ResultData data)
    {
        return (int)data.time;
    }
}
