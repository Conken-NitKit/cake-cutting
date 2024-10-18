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
    [SerializeField] private GameObject serveButton;
    [SerializeField] private GameObject resultButton;

    public int TargetCuttingCount => targetCuttingCount;

    public bool ArrowDrag { get; private set; }

    public bool ArrowCut { get; private set; }

    private bool isCakeOverTargetSlices = false;

    //ケーキを指定された個数以上に分割した時、次の段階に進むボタンを有効にする
    public bool IsCakeOverTargetSlices 
    {
        get
        {
            return isCakeOverTargetSlices;
        }
        set
        {
            if (value)
            {
                serveButton.SetActive(true);
                serveButton.GetComponent<RectTransform>().DOAnchorPosX(0, 0.2f);
            }
            isCakeOverTargetSlices = value;
        }
    }

    private bool isCakeServedOnAllPlates = false;

    //すべての皿に一個以上のケーキを分配した時、結果を表示するボタンを有効にする
    public bool IsCakeServedOnAllPlates
    {
        get
        {
            return isCakeServedOnAllPlates;
        }
        set
        {
            Debug.Log("Recieve:" + value);
            if (value)
            {
                resultButton.SetActive(true);
                resultButton.GetComponent<RectTransform>().DOAnchorPosX(0, 0.2f);
            }
            else
            {
                resultButton.SetActive(false);
                resultButton.GetComponent<RectTransform>().DOAnchorPosX(1000, 0.2f);
            }
            isCakeServedOnAllPlates = value;
        }
    }

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
        serveButton.SetActive(false);
        resultButton.SetActive(false);

    }

    void SetCakeShape(Mesh mesh)
    {
        cake.GetComponent<Mesh2DAssigner>().Mesh2D = Mesh2D.ToMesh2D(mesh).MergeDuplicateVertices().NormalizeSize();
        cake.transform.localPosition = stagedata.DataList[0].CakeOffset;
    }

    public void OnServeButtonClick()
    {
        DistributeCakeToPlates();
        serveButton.GetComponent<RectTransform>().DOAnchorPosX(1000, 0.2f).OnComplete(() => serveButton.SetActive(false));
    }

    public void OnResultButtonClick()
    {
        var sizes = Plate.GetSizes();
        var result =  new ResultData(targetCuttingCount, 0f, sizes, Mathf.Max(Cake.AllCakeMass - sizes.Sum(), 0f), 0);
        ResultDataHandler.Instance.result = result;
        Debug.Log(result);
        Debug.Log(ResultDataHandler.Instance.result);
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
