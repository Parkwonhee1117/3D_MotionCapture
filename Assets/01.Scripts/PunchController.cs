using UnityEngine;

public class PunchController : MonoBehaviour
{
    [Header("Pose Data References")]
    public PoseManager poseManager;         // 미디어파이프 부모

    [Header("Target Settings")]
    public TargetObject targetObject;       // 때려 부술 거대한 구체

    [Header("Punch Settings")]
    public float punchThreshold = 4.0f;     // 펀치 인식 민감도 (숫자가 낮을수록 살짝만 뻗어도 인식)
    public GameObject punchEffectPrefab;   // 주먹 끝에서 터질 이펙트 프리팹

    private Vector3 lastRightHandPos;
    private float lastHitTime = 0f;
    private float cooldownTime = 0.25f;     // 복싱 연타를 위해 쿨타임을 0.25초로 대폭 줄였습니다!

    void Start()
    {
        if (poseManager != null)
        {
            Transform rightHand = poseManager.transform.Find("Landmark_16");
            if (rightHand != null) lastRightHandPos = rightHand.position;
        }
    }

    void Update()
    {
        if (poseManager == null) return;
        
        // 랜드마크 16번(오른손목) 구체 추적
        Transform rightHand = poseManager.transform.Find("Landmark_16");
        if (rightHand == null || !rightHand.gameObject.activeInHierarchy) return;

        // 연타 쿨타임 체크
        if (Time.time < lastHitTime + cooldownTime) return;

        // 🔥 [핵심 변화] 이전 프레임과 현재 프레임의 위치 차이로 "주먹의 순간 속도(가속도)"를 계산합니다.
        Vector3 currentRightHandPos = rightHand.position;
        float handVelocity = (currentRightHandPos - lastRightHandPos).magnitude / Time.deltaTime;

        // 주먹을 지르는 속도가 임계값을 넘으면 펀치 적중으로 판단!
        if (handVelocity > punchThreshold)
        {
            // 🥊 펀치가 나간 방향이 타겟(정면) 쪽일 때만 때리도록 유효 판정
            float forwardVelocity = (currentRightHandPos.z - lastRightHandPos.z) / Time.deltaTime;
            
            // 웹캠을 향해 앞으로 지르거나(Z축), 훅처럼 강하게 휘두를 때 전부 감지
            if (Mathf.Abs(forwardVelocity) > punchThreshold * 0.5f || handVelocity > punchThreshold)
            {
                Debug.Log($"🥊 펀치 적중! 주먹 속도: {handVelocity:F2}");
                
                CastPunchEffect(currentRightHandPos);
                
                if (targetObject != null)
                {
                    targetObject.TakeDamage(1); // 샌드백 때리기
                }

                lastHitTime = Time.time;
            }
        }
        
        lastRightHandPos = currentRightHandPos;
    }

    // 주먹이 꽂힌 좌표에 이펙트 생성
    void CastPunchEffect(Vector3 hitPos)
    {
        if (punchEffectPrefab == null) return;
        GameObject effect = Instantiate(punchEffectPrefab, hitPos, Quaternion.identity);
        Destroy(effect, 1.0f);
    }
}