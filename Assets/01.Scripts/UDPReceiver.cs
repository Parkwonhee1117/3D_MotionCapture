using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    [Header("Network Settings")]
    public int port = 5065;

    [Header("Debug Info")]
    public string lastReceivedPacket = "";

    private UdpClient client;
    private Thread receiveThread;
    private bool isRunning = true; // 스레드 루프를 제어할 변수

    [HideInInspector]
    public Vector3[] landmarkPositions = new Vector3[33];

    void Start()
    {
        isRunning = true; // 시작 시 true로 확실히 초기화
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log($"[UDP] {port}번 포트에서 파이썬 데이터 수신 대기 중...");
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        // [핵심] 타임아웃을 짧게 설정하여 스레드가 종료 신호를 빨리 감지하게 함
        client.Client.ReceiveTimeout = 1000; 
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

        while (isRunning)
        {
            try
            {
                // 소켓이 닫혀있으면 루프 탈출
                if (client == null) break;

                byte[] dataBytes = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(dataBytes);
                lastReceivedPacket = text;
                ParseCoordinates(text);
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.TimedOut)
            {
                // 타임아웃은 정상적인 대기 상태이므로 무시
                continue;
            }
            catch (Exception e)
            {
                if (isRunning) Debug.LogWarning($"[UDP] 수신 중단: {e.Message}");
            }
        }
    }

    private void ParseCoordinates(string data)
    {
        try
        {
            string[] points = data.Split(',');
            if (points.Length >= 99)
            {
                for (int i = 0; i < 33; i++)
                {
                    float x = float.Parse(points[i * 3]);
                    float y = float.Parse(points[i * 3 + 1]);
                    float z = float.Parse(points[i * 3 + 2]);
                    landmarkPositions[i] = new Vector3(x, -y, z);
                }
            }
        }
        catch { }
    }

    // 씬 전환/게임 종료 시 확실하게 정리
    void OnApplicationQuit()
    {
        StopUDP();
    }

    void OnDisable()
    {
        StopUDP();
    }

    private void StopUDP()
    {
        isRunning = false; // 스레드 루프 종료
        
        if (client != null)
        {
            client.Close(); // Receive()를 즉시 해제함
            client = null;
        }

        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join(500); // 스레드가 종료될 때까지 최대 0.5초 대기
            receiveThread.Abort();   // 그래도 안 죽으면 강제 종료
        }
        Debug.Log("[UDP] 네트워크 스레드 및 포트가 안전하게 정리되었습니다.");
    }
}