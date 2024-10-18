using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private GameObject tutorial1;
    [SerializeField] private GameObject tutorial2;

    public bool ShowTutorial;

    public int TargetCuttingCount => targetCuttingCount;

    public bool ArrowDrag { get; private set; }

    public bool ArrowCut { get; private set; }

    public int CuttingCount { get; set; }

    private bool isCakeOverTargetSlices = false;

    private System.Diagnostics.Stopwatch stopwatch;

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
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        ArrowDrag = false;
        ArrowCut = true;
        SetStage(0);
        if (ShowTutorial) ShowTutorial1();

    }

    //その後皿に分ける
    void DistributeCakeToPlates()
    {
        ArrowDrag = true;
        ArrowCut = false;
        table.transform.DOMoveX(10, 0.2f);
        CuttingBoard.CuttingBoardGameObject.transform.DOMoveX(-7, 0.2f);
        if (ShowTutorial) ShowTutorial2();
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
        tutorial1.SetActive(false);
        tutorial2.SetActive(false);

    }

    void SetCakeShape(Mesh mesh)
    {
        cake.GetComponent<Mesh2DAssigner>().Mesh2D = //Mesh2D.ToMesh2D(mesh).MergeDuplicateVertices().NormalizeSize();
            MeshGenerator.GenerateCircleMesh().MergeDuplicateVertices().NormalizeSize();
        cake.transform.localPosition = //stagedata.DataList[0].CakeOffset;
            new Vector3(-5, -5, 0);
    }

    public void OnServeButtonClick()
    {
        DistributeCakeToPlates();
        serveButton.GetComponent<RectTransform>().DOAnchorPosX(1000, 0.2f).OnComplete(() => serveButton.SetActive(false));
    }

    public void OnResultButtonClick()
    {
        stopwatch.Stop();
        var sizes = Plate.GetSizes();
        var result =  new ResultData(targetCuttingCount, (float)stopwatch.Elapsed.TotalSeconds, sizes, Mathf.Max(Cake.AllCakeMass - sizes.Sum(), 0f), CuttingCount);
        ResultDataHandler.Instance.result = result;
        Debug.Log(ResultDataHandler.Instance.result);
        SceneManager.LoadScene("result");
    }

    public void ShowTutorial1()
    {
        tutorial1.SetActive(true);
        var rectTransform = tutorial1.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 0.01f, 1f);
        rectTransform.DOScaleY(1, 0.2f);
        ArrowCut = false;
        ArrowDrag = false;
    }

    public void CloseTutorial1()
    {
        tutorial1.GetComponent<RectTransform>().DOScaleY(0.01f, 0.2f).OnComplete(() =>
        {
            tutorial1.SetActive(false);
            ArrowCut = true;
            ArrowDrag = false;
        });
    }

    public void ShowTutorial2()
    {
        tutorial2.SetActive(true);
        var rectTransform = tutorial2.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 0.01f, 1f);
        rectTransform.DOScaleY(1, 0.2f);
        ArrowCut = false;
        ArrowDrag = false;
    }

    public void CloseTutorial2()
    {
        tutorial2.GetComponent<RectTransform>().DOScaleY(0.01f, 0.2f).OnComplete(() =>
        {
            tutorial2.SetActive(false);
            ArrowCut = false;
            ArrowDrag = true;
        });
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
