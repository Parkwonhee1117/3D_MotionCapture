using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    [Header("Network Settings")]
    public int port = 5065; // 파이썬과 맞춘 대문 번호

    [Header("Debug Info")]
    public string lastReceivedPacket = "";

    // 네트워크 소켓 및 스레드 변수
    private UdpClient client;
    private Thread receiveThread;
    private bool isRunning = true;

    // 외부(다른 스크립트)에서 좌표 데이터를 가져갈 수 있도록 공유하는 배열
    [HideInInspector]
    public Vector3[] landmarkPositions = new Vector3[33]; 

    void Start()
    {
        // 유니티 메인 프레임이 멈추지 않도록 배후(Background)에서 데이터를 받을 스레드 생성
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log($"[UDP] {port}번 포트에서 파이썬 데이터 수신 대기 중...");
    }

    // 5065번 포트를 열고 데이터를 실시간으로 받아내는 백그라운드 함수
    private void ReceiveData()
    {
        try
        {
            client = new UdpClient(port);
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

            while (isRunning)
            {
                // 데이터가 들어올 때까지 대기 (UDP 수신)
                byte[] dataBytes = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(dataBytes);

                lastReceivedPacket = text;

                // 들어온 문자열 파싱 (예: "x,y,z,x,y,z...")
                ParseCoordinates(text);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[UDP] 수신 에러: {e.Message}");
        }
    }

    // 파이썬이 보낸 한 줄짜리 문자열을 유니티가 쓸 수 있는 Vector3 좌표로 쪼개는 작업
    private void ParseCoordinates(string data)
    {
        try
        {
            string[] points = data.Split(',');
            
            // 33개의 관절이므로 x, y, z 세트 계산 (33 * 3 = 99개 데이터)
            if (points.Length >= 99)
            {
                for (int i = 0; i < 33; i++)
                {
                    float x = float.Parse(points[i * 3]);
                    float y = float.Parse(points[i * 3 + 1]);
                    float z = float.Parse(points[i * 3 + 2]);

                    // 유니티 좌표계에 맞게 할당 (필요시 배율 조절 가능)
                    // 파이썬과 유니티의 Y축 상하 반전을 해결하기 위해 y에 -를 붙이기도 합니다.
                    landmarkPositions[i] = new Vector3(x, -y, z); 
                }
            }
        }
        catch (Exception e)
        {
            // 데이터가 순간적으로 깨져서 들어올 때의 예외 처리
        }
    }

    // 게임이 꺼질 때 포트와 스레드를 안전하게 닫아줍니다. (안 하면 포트가 잠김)
    void OnApplicationQuit()
    {
        isRunning = false;
        if (client != null) client.Close();
        if (receiveThread != null && receiveThread.IsAlive) receiveThread.Abort();
        Debug.Log("[UDP] 수신 스레드가 안전하게 종료되었습니다.");
    }

    void OnDisable() // 씬이 종료되거나 오브젝트가 비활성화될 때 실행
{
    if (client != null)
    {
        client.Close(); // 포트 점유 해제
        Debug.Log("UDP 포트가 안전하게 닫혔습니다.");
    }
}
}