using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CuttingFlowManager : MonoBehaviour
{
    [SerializeField] private int targetCuttingCount = 4;
    [SerializeField] private SlidePlates slidePlates;
    [SerializeField] private StageSetting stagedata;
    [SerializeField] private GameObject cake;
    [SerializeField] private GameObject table;
    [SerializeField] private TextMeshProUGUI targetText;

    public bool ArrowDrag { get; private set; }

    public bool ArrowCut { get; private set; }

    public static CuttingFlowManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    //ステージを読み込む
    void Start()
    {
        ArrowDrag = false;
        ArrowCut = true;
        SetStage(0);
    }

    //その後皿に分ける
    void DistributeCakeToPlates()
    {
        ArrowDrag = true;
        ArrowCut = false;
        table.transform.DOMoveX(10, 0.2f);
        CuttingBoard.CuttingBoardGameObject.transform.DOMoveX(-7, 0.2f);
    }

    void SetStage(int id)
    {
        targetCuttingCount = stagedata.DataList[id].TargetCuttingCount;
        table.transform.position = new Vector3(25, 0, 0);
        CuttingBoard.CuttingBoardGameObject.transform.position = new Vector3(0, -1f, 0);
        slidePlates.Setting(targetCuttingCount);
        targetText.text = "Target:" + targetCuttingCount;
        SetCakeShape(stagedata.DataList[id].CakeShape);
    }

    void SetCakeShape(Mesh mesh)
    {
        cake.GetComponent<Mesh2DAssigner>().Mesh2D = Mesh2D.ToMesh2D(mesh).MergeDuplicateVertices().NormalizeSize();
        cake.transform.localPosition = stagedata.DataList[0].CakeOffset;
    }

    public void OnServeButtonClick()
    {
        DistributeCakeToPlates();
    }

    public void OnResultButtonClick()
    {
        var sizes = Plate.GetSizes();
        var result =  new ResultData(targetCuttingCount, 0f, sizes, Cake.AllCakeMass - sizes.Sum(), 0);
        ResultDataHandler.Instance.result = result;
        Debug.Log(result);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnServeButtonClick();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnResultButtonClick();
        }
    }
}
