using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    private float timer = 0f;

    void Start()
    {
        timer = 0f; // 씬 시작 시 시간 초기화
    }

    void Update()
    {
        if (!TargetObject.isGameOver) // 게임 중일 때만 측정
        {
            timer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}