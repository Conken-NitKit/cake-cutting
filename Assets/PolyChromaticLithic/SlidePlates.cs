using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlidePlates : MonoBehaviour
{
    public float plateSize = 9;
    public int count = 4;
    private int current = 0;
    [SerializeField] private float horizontalSize = 8f;
    [SerializeField] private float verticalSize = 12f;
    [SerializeField] private float slideTime = 0.5f;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject tableCloth;
    [SerializeField] private GameObject platePrefab;

    private bool isSliding = false;
    private float x = 0;

    private Tween tween;

    public int Current
    {
        get => current;
        set
        {
            current = value;
            text.text = (current + 1).ToString();
        }
    }

    private void Start()
    {
        Current = 0;
        x = transform.position.x;
        SetPlates();
        SetTableCloth();
    }

    private void SetPlates()
    {
        for (int i = 0; i < count; i++)
        {
            var plate = Instantiate(platePrefab, transform);
            plate.transform.position = new Vector3(x, i * plateSize, 0);
        }
    }

    private void SetTableCloth()
    {
        //‚¢‚ë‚¢‚ëŽŽ‚µ‚½Œ‹‰Ê‚Ì’l
        tableCloth.transform.position = new Vector3(x, count * 3.4f, 0);
        tableCloth.GetComponent<SpriteRenderer>().size = new Vector2(plateSize / 4.09f, (count - 1) * 1.833f + 2.5f);
    }

    private void Update()
    {
        if (isSliding) { return; }
        x = transform.position.x;
        var cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (cursorPos.x < horizontalSize / 2 + x && cursorPos.x > -horizontalSize / 2 + x)
        {
            if (cursorPos.y > verticalSize / 2)
            {
                if (!isSliding)
                {
                    isSliding = true;
                    Current = (Current + 1) % count;
                    tween = transform.DOMoveY(-Current * plateSize, slideTime).SetEase(Ease.InOutSine).OnComplete(() => isSliding = false);
                }
            }
            else if (cursorPos.y < -verticalSize / 2)
            {
                if (!isSliding)
                {
                    isSliding = true;
                    Current = (Current - 1 + count) % count;
                    tween = transform.DOMoveY(-Current * plateSize, slideTime).SetEase(Ease.InOutSine).OnComplete(() => isSliding = false);
                }
            }

        }
    }

    public void OnUpButtonClick()
    {
        isSliding = true;
        Current = (Current + 1) % count;
        tween.Kill();
        tween = transform.DOMoveY(-Current * plateSize, slideTime * 0.5f).SetEase(Ease.InOutSine).OnComplete(() => isSliding = false);
    }

    public void OnDownButtonClick()
    {
        isSliding = true;
        Current = (Current - 1 + count) % count;
        tween.Kill();
        tween = transform.DOMoveY(-Current * plateSize, slideTime * 0.5f).SetEase(Ease.InOutSine).OnComplete(() => isSliding = false);
    }
}
