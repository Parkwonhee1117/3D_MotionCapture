using UnityEngine;

public class PunchController : MonoBehaviour
{
    [Header("References")]
    public PoseManager poseManager;
    public TargetObject targetObject;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] punchClips;

    [Header("Punch Settings")]
    public float punchThreshold = 4.0f;
    public GameObject punchEffectPrefab;
    
    [Header("Damage Settings")]
    public float baseDamage = 5f;
    public float speedFactor = 2.0f;

    private bool isTouchingTarget = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Punch"))
        {
            isTouchingTarget = true;
            // 닿는 순간 바로 타격 처리
            PerformPunch(punchThreshold * 1.5f, other.transform.position);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Punch"))
        {
            isTouchingTarget = false;
        }
    }

    void PerformPunch(float velocity, Vector3 hitPos)
    {
        // 1. 데미지 계산
        int damage = Mathf.RoundToInt(baseDamage + (velocity * speedFactor));
        if (targetObject != null) targetObject.TakeDamage(damage);

        // 2. 랜덤 사운드 재생
        if (audioSource != null && punchClips.Length > 0)
        {
            int randomIndex = Random.Range(0, punchClips.Length);
            audioSource.clip = punchClips[randomIndex];
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();
        }

        // 3. 이펙트 생성
        if (punchEffectPrefab != null)
        {
            GameObject effect = Instantiate(punchEffectPrefab, hitPos, Quaternion.identity);
            Destroy(effect, 1.0f);
        }

        Debug.Log($"🥊 타격 성공! 데미지: {damage}");
    }
}