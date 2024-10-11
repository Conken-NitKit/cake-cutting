using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cake : MonoBehaviour
{
    SpriteRenderer MainSpriteRenderer;
    [SerializeField] private List<Sprite> kind = new List<Sprite>();
    [SerializeField] private float speed;

    void Start()
    {
        MainSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        choice_cake();
    }

    // Update is called once per frame
    void Update()
    {

            Transform tr = this.transform;
            tr.Translate(-speed * Time.deltaTime, 0, 0);
            if (tr.localPosition.x < -15.0f)
            {
                tr.Translate(30.0f, 0.0f, 0.0f, Space.World);
            choice_cake();
            }
    }


    void choice_cake()
    {
        Sprite maincake;
        int rnd = Random.Range(0, 15);
        maincake = kind[rnd];
        MainSpriteRenderer.sprite = maincake;
    }
    public void OnTouched(){
    }
}
