using UnityEngine;

public class TargetObject : MonoBehaviour
{
    [Header("Sandbag Settings")]
    public int maxHp = 20;                 // 복싱 연타용이므로 피통을 20으로 넉넉히 잡았습니다.
    private int currentHp;

    [Header("Effects")]
    public GameObject destroyEffectPrefab; // 완파 시 터질 큰 파티클

    // ⏱️ 타임어택 시스템 변수
    private float startTime;
    private bool isGamePlaying = false;
    private bool isCleared = false;

    private Vector3 originalScale;

    void Start()
    {
        currentHp = maxHp;
        originalScale = transform.localScale;
        
        // 게임 시작 시간 기록
        startTime = Time.time;
        isGamePlaying = true;
        Debug.Log("🥊 타임어택 시작! 샌드백을 최대한 빠르게 연타하세요!");
    }

    void Update()
    {
        // 게임 중일 때 콘솔에 실시간 흐른 시간 표시 (선택 사항)
        if (isGamePlaying && !isCleared)
        {
            float elapsedTime = Time.time - startTime;
            // 필요하다면 UI Text에 elapsedTime.ToString("F2")를 연동하면 화면에 실시간으로 초가 올라갑니다!
        }
    }

    // 타격받았을 때 실행되는 함수
    public void TakeDamage(int damage)
    {
        if (isCleared) return;

        currentHp -= damage;
        Debug.Log($"🥊 샌드백 피격! 남은 HP: {currentHp}");

        // 💥 타격감 연출: 펀치 맞을 때마다 좌우로 흔들리거나 움찔하는 연출
        transform.localScale = originalScale * 0.85f;
        Invoke("ResetScale", 0.05f);

        // HP가 0이 되면 쓰러짐(클리어)!
        if (currentHp <= 0)
        {
            ClearGame();
        }
    }

    void ResetScale()
    {
        transform.localScale = originalScale;
    }

    // 🏆 샌드백을 쓰러뜨렸을 때 실행되는 함수
    void ClearGame()
    {
        isCleared = true;
        isGamePlaying = false;

        // ⏱️ 최종 기록 계산 (현재 시간 - 시작 시간)
        float clearTime = Time.time - startTime;

        Debug.Log("=========================================");
        Debug.Log($"🎉 샌드백 격파 완료!! KO!");
        Debug.Log($"⏱️ 최종 클리어 기록: [ {clearTime:F2}초 ] 걸렸습니다!");
        Debug.Log("=========================================");

        // 파괴 이펙트 터트리기
        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        }

        // 샌드백이 쓰러진 연출로 비활성화하거나 바닥으로 날려버림
        gameObject.SetActive(false); 
    }
}