import sys
import cv2
import socket
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision

print("1. OpenCV 및 통신 라이브러리 로드 성공!")

UDP_IP = "127.0.0.1"
UDP_PORT = 5065
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# 같은 폴더 안에 있는 모델을 정확히 바라봅니다
model_path = 'pose_landmarker.task'

try:
    base_options = python.BaseOptions(model_asset_path=model_path)
    options = vision.PoseLandmarkerOptions(base_options=base_options, output_segmentation_masks=False)
    detector = vision.PoseLandmarker.create_from_options(options)
    print("🎉 2. M4 맥북 최적화 AI 엔진 초기화 대성공!")
except Exception as e:
    print(f"❌ 엔진 초기화 실패: {e}")
    sys.exit()

cap = cv2.VideoCapture(0)
print(f"📸 3. 웹캠 작동 중... (유니티 포트 {UDP_PORT}번으로 데이터 송신 시작)")

while cap.isOpened():
    success, image = cap.read()
    if not success: continue

    image_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=image_rgb)
    detection_result = detector.detect(mp_image)

    if detection_result.pose_landmarks:
        for landmark_list in detection_result.pose_landmarks:
            data_string = ""
            for lm in landmark_list:
                data_string += f"{lm.x},{1 - lm.y},{lm.z},"
            data_string = data_string[:-1]
            
            # 유니티로 전송
            sock.sendto(data_string.encode(), (UDP_IP, UDP_PORT))
            
            # 초록색 관절 점 시각화
            for lm in landmark_list:
                h, w, _ = image.shape
                cx, cy = int(lm.x * w), int(lm.y * h)
                cv2.circle(image, (cx, cy), 5, (0, 255, 0), -1)
        
        print(f"유니티로 좌표 송신 중... (데이터 길이: {len(data_string)})", end="\r")

    cv2.imshow('Wonhee M4 Mocap -> Unity', image)
    if cv2.waitKey(5) & 0xFF == ord('q'): break

cap.release()
cv2.destroyAllWindows()
print("\n👋 서버가 정상 종료되었습니다.")