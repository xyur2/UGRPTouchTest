using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    private string state = "Idle";
    public Button RedDot;
    public Component cross;
    public int IterTimes = 3;
    public Button ExportCSV;
    private bool isRedDotClicked = false;
    public float buttonScale = 10f;
    public int maxCount = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RedDot.onClick.AddListener(OnRedDotClick);
        RedDot.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        RedDot.gameObject.GetComponent<CanvasGroup>().interactable = false;
        ExportCSV.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        ExportCSV.gameObject.GetComponent<CanvasGroup>().interactable = false;
        state = "start";
    }

    // Update is called once per frame
    void Update()
    {
        string SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (state == "start")
        {
            if (SceneName == "TouchTest"){
                StartCoroutine(TouchTest());
            }
        }
        if (state == "TouchTestEnd")
        {
            ExportCSV.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            ExportCSV.gameObject.GetComponent<CanvasGroup>().interactable = true;
        }
    }
    
    IEnumerator TouchTest()
    {
        state = "TouchTest";
        Debug.Log("TouchTest");
        // 6x6 그리드의 터치 위치를 나타내는 배열 생성
        int iter_now = 1;
        RedDot.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonScale, buttonScale);
        while (iter_now < IterTimes)
        {
            Vector2[] touchPositions = new Vector2[36];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    touchPositions[i * 6 + j] = new Vector2(i, j);
                }
            }
            // 배열 셔플
            for (int i = touchPositions.Length - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                Vector2 temp = touchPositions[i];
                touchPositions[i] = touchPositions[randomIndex];
                touchPositions[randomIndex] = temp;
            }
            int trialCount = 0;
            int maxTrials = maxCount;
            foreach (var pos in touchPositions)
            {
                Debug.Log(pos);
                if (trialCount >= maxTrials) break;
                RedDot.transform.position = new Vector3(pos.x*320 + 160, pos.y*180 + 90, 0);
                cross.transform.position = new Vector3(pos.x*320 + 160, pos.y*180 + 90, 0);
                RedDot.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
                RedDot.gameObject.GetComponent<CanvasGroup>().interactable = true;
                Debug.Log(RedDot.transform.position);
                while (!isRedDotClicked)
                {
                    yield return null;
                }
                isRedDotClicked = false;
                trialCount += 1;
            }
            iter_now += 1;
        }
        state = "TouchTestEnd";
    }
    void OnRedDotClick()
    {
        Debug.Log("RedDot Clicked");
        isRedDotClicked = true;
        RedDot.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        RedDot.gameObject.GetComponent<CanvasGroup>().interactable = false;
    }
}
