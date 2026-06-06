using UnityEngine;

public class PoseManager : MonoBehaviour
{
    public UDPReceiver udpReceiver;
    public GameObject landmarkPrefab;
    public float multiplier = 1.0f; 
    public Material lineMaterial;

    private GameObject[] landmarks = new GameObject[33];
    private LineRenderer[] lines = new LineRenderer[35];
    private int[,] connections = new int[,] {
        {0,1},{1,2},{2,3},{3,7},{0,4},{4,5},{5,6},{6,8},{9,10},
        {11,12},{11,13},{13,15},{15,17},{15,19},{15,21},{17,19},
        {12,14},{14,16},{16,18},{16,20},{16,22},{18,20},{11,23},
        {12,24},{23,24},{23,25},{25,27},{27,29},{29,31},{31,27},
        {24,26},{26,28},{28,30},{30,32},{32,28}
    };

    void Start()
    {
        for (int i = 0; i < 33; i++)
        {
            landmarks[i] = Instantiate(landmarkPrefab ? landmarkPrefab : GameObject.CreatePrimitive(PrimitiveType.Sphere), transform);
            landmarks[i].transform.localScale = Vector3.one * 0.05f;
        }

        for (int i = 0; i < 35; i++)
        {
            GameObject lineObj = new GameObject("Line_" + i);
            lineObj.transform.parent = transform;
            lines[i] = lineObj.AddComponent<LineRenderer>();
            lines[i].startWidth = 0.02f; lines[i].endWidth = 0.02f;
            lines[i].material = lineMaterial;
        }
    }

    void LateUpdate()
    {
        if (udpReceiver == null || udpReceiver.landmarkPositions == null) return;
        Vector3[] rawPoints = udpReceiver.landmarkPositions;
        
        // 데이터의 중심점(골반)을 구함
        Vector3 hipCenter = (rawPoints[23] + rawPoints[24]) * 0.5f;

        for (int i = 0; i < 33; i++)
        {
            // 💡 여기서 중심점을 빼고 멀티플라이어를 곱한 뒤, 좌표계를 0,0,0으로 맞춤
            Vector3 pos = (rawPoints[i] - hipCenter) * multiplier;
            
            // 유니티 월드 좌표로 변환 (상하 반전 해결)
            landmarks[i].transform.localPosition = new Vector3(-pos.x, -pos.y, pos.z);
        }

        for (int i = 0; i < 35; i++)
        {
            lines[i].SetPosition(0, landmarks[connections[i, 0]].transform.position);
            lines[i].SetPosition(1, landmarks[connections[i, 1]].transform.position);
        }
    }
}