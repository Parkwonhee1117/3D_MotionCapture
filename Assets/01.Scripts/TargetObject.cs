using UnityEngine;

public class TargetObject : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;
    public RectTransform hpBarFill;
    public GameObject clearUI;
    public static bool isGameOver = false; // 게임 상태 변수

    void Start()
    {
        currentHP = maxHP;
        isGameOver = false; // 시작할 때 게임 상태 초기화
        if (clearUI != null) clearUI.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        if (isGameOver) return; // 게임 끝났으면 데미지 안 받음

        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        float ratio = currentHP / maxHP;
        if (hpBarFill != null) hpBarFill.localScale = new Vector3(ratio, 1, 1);

        if (currentHP <= 0)
        {
            GameClear();
        }
    }

    void GameClear()
    {
        isGameOver = true; // 게임 종료 상태로 전환
        if (clearUI != null) clearUI.SetActive(true);
        
        // 점수 저장
        PlayerPrefs.SetInt("LastScore", (int)currentHP); 
        PlayerPrefs.Save();
        
        // 주의: Time.timeScale = 0; 대신 isGameOver 변수로 로직을 막습니다.
    }
}