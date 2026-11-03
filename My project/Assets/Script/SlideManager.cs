using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // PointerUp 감지를 위해 필수

public class SlideManager : MonoBehaviour
{
    private string state = "Idle";
    
    [Header("UI Elements")]
    public Button RedButtonStart;  // 시작 지점 버튼
    public Image EndButton;        // 도착 지점 이미지
    public Image Bridge;           // 두 지점을 잇는 선 이미지
    public Button ExportCSV;       // 결과 출력 버튼
    public int trialsnum = 100;
    
    private bool isPointerReleasedOnEnd = false;
    public float bridgeHeight = 20f; // 선의 두께

    void Start()
    {
        // 1. EndButton에 EventTrigger 추가 (손가락을 뗐을 때 감지)
        SetupPointerUpEvent();

        // 2. 초기 UI 상태 설정 (모두 숨김)
        SetAlpha(RedButtonStart.gameObject, 0f);
        SetAlpha(EndButton.gameObject, 0f);
        SetAlpha(Bridge.gameObject, 0f);
        SetAlpha(ExportCSV.gameObject, 0f);

        state = "start";
    }

    void Update()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        // "SlideTest" 씬에서만 코루틴 시작
        if (state == "start" && sceneName == "SlideTest")
        {
            StartCoroutine(SlideTest());
        }

        // 테스트 종료 시 CSV 내보내기 버튼 활성화
        if (state == "SlideTestEnd")
        {
            SetAlpha(ExportCSV.gameObject, 1f, true);
        }
    }

    // EventTrigger를 통해 PointerUp(손을 뗌) 감지 로직 등록
    void SetupPointerUpEvent()
    {
        EventTrigger trigger = EndButton.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = EndButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    void OnPointerUp(PointerEventData data)
    {
        // 손을 뗀 지점이 EndButton 영역 내부라면 성공
        isPointerReleasedOnEnd = true;
    }

    IEnumerator SlideTest()
    {
        state = "SlideTest";
        Debug.Log("SlideTest 시작");

        // 1. 6x6 그리드 좌표 설정 (1920x1080 해상도 기준 예시)
        Vector2[] positions = new Vector2[36];
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                positions[i * 6 + j] = new Vector2(i * 320 + 160, j * 180 + 90);
            }
        }

        // 2. 36 * 35개의 모든 무작위 순서쌍 조합 생성 (1,260개)
        List<Vector2Int> combinations = new List<Vector2Int>();
        for (int i = 0; i < 36; i++)
        {
            for (int j = 0; j < 36; j++)
            {
                if (i == j) continue; // 시작과 끝이 같은 자리는 제외
                if (System.Math.Abs(i%6 - j%6) > 2) continue;
                if (System.Math.Abs(i-j) > 5) continue;

                combinations.Add(new Vector2Int(i, j));
            }
        }

        // 3. 조합 무작위 셔플 (Fisher-Yates Shuffle)
        for (int i = combinations.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            Vector2Int temp = combinations[i];
            combinations[i] = combinations[rnd];
            combinations[rnd] = temp;
        }
        // int trialCount = 0;
        
        // 최대 100회 반복 (조합이 너무 많으므로 제한)
        // int maxTrials = trialsnum; 
        
        // 4. 테스트 루프 진행
        foreach (var pair in combinations)
        {
            // if (trialCount >= maxTrials) break;

            // 위치 배치 및 선 연결
            PlaceElements(positions[pair.x], positions[pair.y]);

            // UI 표시 및 상호작용 활성화
            SetAlpha(RedButtonStart.gameObject, 1f);
            SetAlpha(EndButton.gameObject, 1f, true);
            SetAlpha(Bridge.gameObject, 1f);

            // 사용자가 EndButton 위에서 손을 뗄 때까지 대기
            while (!isPointerReleasedOnEnd)
            {
                yield return null;
            }

            // 플래그 리셋 및 UI 숨김
            isPointerReleasedOnEnd = false;
            
            SetAlpha(RedButtonStart.gameObject, 0f);
            SetAlpha(EndButton.gameObject, 0f, false);
            SetAlpha(Bridge.gameObject, 0f);
            
            // 다음 문제로 넘어가기 전 짧은 대기 시간
            // trialCount += 1;
            yield return new WaitForSeconds(0.3f); 
        }

        state = "SlideTestEnd";
    }

    // 수학적 계산을 통한 두 버튼 및 브릿지 배치
    void PlaceElements(Vector2 start, Vector2 end)
    {
        // 시작 및 종료 지점 위치 설정
        RedButtonStart.GetComponent<RectTransform>().position = start;
        EndButton.rectTransform.position = end;

        // 방향 벡터, 거리, 각도 계산
        Vector2 dir = end - start;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Bridge 이미지 배치: 두 지점의 중앙에 위치시키고, 길이를 거리만큼 늘린 뒤 회전
        Bridge.rectTransform.position = (start + end) / 2f;
        Bridge.rectTransform.sizeDelta = new Vector2(distance, bridgeHeight);
        Bridge.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // CanvasGroup을 이용한 UI 투명도 및 클릭 가능 여부 제어 유틸리티
    void SetAlpha(GameObject obj, float alpha, bool interactable = false)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        
        cg.alpha = alpha;
        cg.interactable = interactable;
        cg.blocksRaycasts = interactable; // alpha가 0일 때 클릭 방지
    }
}