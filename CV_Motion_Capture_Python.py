import cv2
import mediapipe as mp

# MediaPipe에서 관절(Pose)을 찾기 위한 부품들 가져오기
mp_pose = mp.solutions.pose
mp_drawing = mp.solutions.drawing_utils
pose = mp_pose.Pose(min_detection_confidence=0.5, min_tracking_confidence=0.5)

# 컴퓨터에 연결된 기본 웹캠 열기 (0번은 내장/기본 웹캠)
cap = cv2.VideoCapture(0)

while cap.isOpened():
    success, image = cap.read()
    if not success:
        print("웹캠을 찾을 수 없습니다.")
        continue

    # 1. MediaPipe 처리를 위해 이미지 색상 상을 BGR에서 RGB로 변환
    image_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    results = pose.process(image_rgb)

    # 2. 화면에 관절 포인트와 연결선 그리기
    if results.pose_landmarks:
        mp_drawing.draw_landmarks(
            image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS
        )
        
        # [확인용] 오른쪽 어깨(ID: 12)의 3D 좌표 출력해보기
        # MediaPipe는 총 33개의 관절 좌표(results.pose_landmarks.landmark)를 제공합니다.
        right_shoulder = results.pose_landmarks.landmark[12]
        print(f"오른쪽 어깨 좌표 -> X: {right_shoulder.x:.2f}, Y: {right_shoulder.y:.2f}, Z: {right_shoulder.z:.2f}")

    # 3. 결과를 화면에 보여주기
    cv2.imshow('MediaPipe Motion Capture Test', image)

    # 'q' 키를 누르면 종료
    if cv2.waitKey(5) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()