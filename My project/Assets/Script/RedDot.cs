using UnityEngine;
using UnityEngine.UI;

public class RedDot : MonoBehaviour
{
    public Button myButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myButton.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void OnButtonClick()
    {
        Debug.Log("Button Clicked");
    }
}
