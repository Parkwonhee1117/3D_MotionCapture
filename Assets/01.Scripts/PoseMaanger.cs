using UnityEngine;

public class PoseManager : MonoBehaviour
{
    // 아까 만든 UDPReceiver 스크립트를 연결할 변수
    public UDPReceiver UDPReceiver;

    // 관절을 표시할 3D 구체(Sphere) 프리팹 또는 원본 오브젝트
    public GameObject LandmarkPrefab;

    // 생성된 33개의 구체들을 담아둘 배열
    private GameObject[] landmarks = new GameObject[33];

    [Header("Scale Settings")]
    public float multiplier = 10f; // 파이썬 좌표계(0~1)를 유니티 크기에 맞게 키워주는 배율

    void Start()
    {
        // 1. Scene에 landmarkPrefab이 설정되어 있는지 확인
        if (LandmarkPrefab == null)
        {
            // 만약 에디터에서 등록 안 했으면 기본 유니티 구체(Sphere)를 생성해서 씁니다.
            LandmarkPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            LandmarkPrefab.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            LandmarkPrefab.SetActive(false); // 원본은 잠시 꺼둡니다.
        }

        // 2. 33개의 관절 구체를 미리 생성(Instantiate)해서 배열에 배치합니다.
        for (int i = 0; i < 33; i++)
        {
            landmarks[i] = Instantiate(LandmarkPrefab, Vector3.zero, Quaternion.identity);
            landmarks[i].name = $"Landmark_{i}";
            landmarks[i].SetActive(true);
            
            // 이 스크립트가 붙은 오브젝트 하위로 정렬해서 Hierarchy를 깔끔하게 유지합니다.
            landmarks[i].transform.parent = this.transform; 
        }
    }

    void Update()
    {
        // 안전장치: UDPReceiver가 없거나 데이터가 아직 안 들어왔다면 패스
        if (UDPReceiver == null || UDPReceiver.landmarkPositions == null) return;

        // 3. 매 프레임마다 파이썬에서 들어온 좌표를 구체들의 위치에 실시간 대입합니다.
        for (int i = 0; i < 33; i++)
        {
            // UDPReceiver에서 파싱된 좌표를 가져와 배율(multiplier)을 곱해줍니다.
            Vector3 rawPos = UDPReceiver.landmarkPositions[i];

            Vector3 targetPos = new Vector3(-rawPos.x, -rawPos.y, rawPos.z) * multiplier;

            // 유니티 3D 공간에 좌표 반영
            landmarks[i].transform.localPosition = targetPos;
        }
    }
}