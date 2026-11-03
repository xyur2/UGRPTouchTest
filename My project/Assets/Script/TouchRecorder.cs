using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TouchData
{
    public float time;
    public int touchId;
    public string phase;
    public float posX;
    public float posY;
    public bool clicked;
    public float buttonX;
    public float buttonY;
    public float button2X;
    public float button2Y;

    public TouchData(Touch t, bool c = false, float bX = 0, float bY = 0, float b2X = 0, float b2Y = 0)
    {
        time = Time.time;
        touchId = t.fingerId;
        phase = t.phase.ToString();
        posX = t.position.x;
        posY = t.position.y;
        clicked = c;
        buttonX = bX;
        buttonY = bY;
        button2X = b2X;
        button2Y = b2Y;
    }

    // CSV 한 줄 형식으로 변환
    public string ToCsvRow()
    {
        return $"{time},{touchId},{phase},{posX},{posY},{clicked},{buttonX},{buttonY},{button2X},{button2Y}";
    }
}

public class TouchRecorder : MonoBehaviour
{
    private List<TouchData> recordedTouches = new List<TouchData>();
    private bool touched;
    public Button myButton;
    public Button myButton2;
    void Start()
    {
        myButton.onClick.AddListener(OnButtonClick);
    }

    void Update()
    {
        // 화면에 터치가 있을 때마다 기록
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                recordedTouches.Add(new TouchData(touch, touched, myButton.transform.position.x, myButton.transform.position.y,myButton2.transform.position.x, myButton2.transform.position.y));
                touched = false;
            }
        }
    }

    public void SaveToCSV()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "TouchLog.csv");
        StringBuilder sb = new StringBuilder();

        // 헤더 추가
        sb.AppendLine("Time,TouchID,Phase,PosX,PosY,Clicked,Button1X,Button1Y,Button2X,Button2Y");

        // 데이터 추가
        foreach (var data in recordedTouches)
        {
            sb.AppendLine(data.ToCsvRow());
        }

        // 파일 쓰기
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        Debug.Log($"CSV 파일이 저장되었습니다: {filePath}");
    }

    public void OnButtonClick()
    {
        touched = true;
    }
}