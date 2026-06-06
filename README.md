# 🥊 3D Motion Capture Boxing Game

MediaPipe 기반 실시간 모션 캡처 기술을 활용한 인터랙티브 3D 복싱 게임입니다.

Python에서 추출한 인체 관절 좌표 데이터를 UDP 통신을 통해 Unity로 실시간 전송하고, 이를 기반으로 플레이어 아바타를 제어하여 샌드백을 타격하는 물리 기반 게임 시스템을 구현했습니다.

---

## 📌 프로젝트 개요

본 프로젝트는 컴퓨터 비전(Computer Vision) 기술과 게임 엔진을 결합하여 사용자의 실제 움직임을 게임 내 아바타에 반영하는 실시간 모션 캡처 시스템을 구현하는 것을 목표로 제작되었습니다.

MediaPipe Pose를 이용해 웹캠 영상에서 인체 관절 위치를 추출하고, UDP 네트워크 통신을 통해 Unity 환경으로 전달하여 플레이어의 움직임을 실시간으로 동기화합니다.

---

## 🎮 주요 기능

### 🧍 실시간 모션 캡처

* MediaPipe Pose를 활용한 실시간 인체 관절 추적
* 웹캠 영상으로부터 33개 랜드마크 좌표 추출
* 플레이어 움직임을 Unity 아바타에 실시간 반영

### 📡 UDP 기반 데이터 통신

* Python ↔ Unity 간 UDP 소켓 통신 구현
* 최소 지연시간(Latency)을 고려한 실시간 데이터 전송
* 실시간 관절 좌표 동기화

### 🥊 물리 기반 타격 시스템

* 손 위치 및 이동 속도를 기반으로 펀치 판정
* 펀치 속도에 비례한 데미지 계산
* 타격 시 물리 효과 및 사운드 효과 적용

### ❤️ 게임 시스템

* 샌드백 체력(HP) 관리
* 체력바 UI 연동
* KO(Game Clear) 판정 시스템
* 게임 재시작 및 종료 기능

---

## 🛠️ 개발 환경

### Unity

* Unity Engine
* C#

### Python

* Python 3.x
* MediaPipe
* OpenCV

### Networking

* UDP Socket Communication

---

## 📦 설치 방법

### 1. Python 환경 설정

프로젝트 내 Python 폴더에서 필요한 라이브러리를 설치합니다.

```bash
pip install mediapipe opencv-python
```

또는 requirements.txt를 사용하는 경우:

```bash
pip install -r requirements.txt
```

---

## 🚀 실행 방법

### Step 1. Python 서버 실행

Python 폴더 내 메인 스크립트를 실행합니다.

```bash
python main.py
```

기본적으로 UDP 포트 **5065**를 사용합니다.

### Step 2. Unity 실행

1. Unity 프로젝트를 실행합니다.
2. Play 버튼을 눌러 게임을 시작합니다.

### Step 3. 게임 플레이

* 웹캠이 사용자의 전신 또는 상반신을 인식합니다.
* 사용자의 움직임이 게임 내 아바타에 반영됩니다.
* 주먹을 휘둘러 샌드백을 타격하고 체력을 감소시킵니다.
* 샌드백의 체력이 모두 소진되면 KO가 발생하며 게임이 종료됩니다.

---

## 📂 프로젝트 구조

```text
Project
├── Assets
│   ├── Scripts
│   │   ├── UDPReceiver
│   │   ├── PoseManager
│   │   ├── PunchDetector
│   │   ├── HPManager
│   │   └── UIManager
│   │
│   └── Prefabs
│
├── Python
│   ├── main.py
│   └── pose_sender.py
│
└── requirements.txt
```

---

## 🔄 시스템 구조

```text
WebCam
   │
   ▼
MediaPipe Pose (Python)
   │
   ▼
UDP Socket Communication
   │
   ▼
Unity UDP Receiver
   │
   ▼
Avatar Motion Update
   │
   ▼
Punch Detection
   │
   ▼
Damage Calculation
   │
   ▼
HP UI & KO System
```

---

## ⚙️ 주요 구현 내용

### 실시간 모션 데이터 처리

* MediaPipe Pose를 이용한 관절 좌표 추출
* 33개 랜드마크 데이터 실시간 전송
* Unity 내 아바타 관절 위치 동기화

### 타격 판정 시스템

* 손 이동 속도 기반 펀치 감지
* 충돌 시 데미지 계산
* 타격 효과 및 피드백 제공

### 게임 상태 관리

* HP 감소 및 UI 갱신
* 게임 종료 상태(Game Over) 관리
* 재시작 및 종료 기능 제공

---

## 📝 Troubleshooting

### Address already in use 오류

씬 종료 또는 게임 종료 시 UDP 소켓이 정상적으로 해제되지 않으면 발생할 수 있습니다.

해결 방법:

* `OnApplicationQuit()`
* `OnDisable()`

에서 UDP 소켓과 수신 스레드를 명시적으로 종료하도록 구현했습니다.

### 씬 재시작 시 데이터 수신 오류

씬이 다시 로드될 때 이전 스레드가 남아있어 발생할 수 있습니다.

해결 방법:

* UDP 스레드 생명주기 관리
* 씬 전환 시 안전한 자원 해제 처리

### 게임 종료 후 아바타가 계속 움직이는 문제

해결 방법:

* `isGameOver` 플래그를 활용하여 게임 종료 시 위치 업데이트를 차단
* KO 연출 중에도 안정적인 상태 유지

---

## 🎥 시연 영상


[📺 시연 영상 보기]

![Uploading Video Project 2.gif…]()

---



## 👨‍💻 개발자

박원희

Computer Vision · Unity · Real-Time Networking · Game Development
