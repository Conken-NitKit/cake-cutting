using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class setting_button : MonoBehaviour
{
    // Start is called before the first frame update
    Button button;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnPlayButtonClick());
    }

    void OnPlayButtonClick()
    {
        Debug.Log("ê›íËâÊñ ");
        //SceneManager.LoadScene("");
    }
}
