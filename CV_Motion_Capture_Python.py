import sys
import cv2 as cv
import socket
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision

print("1. OpenCV 및 통신 라이브러리 로드 성공!")

UDP_IP = "127.0.0.1"
UDP_PORT = 5065
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

model_path = 'pose_landmarker.task'

# 모델 초기화 (try-except 유지)
try:
    base_options = python.BaseOptions(model_asset_path=model_path)
    options = vision.PoseLandmarkerOptions(base_options=base_options, output_segmentation_masks=False)
    detector = vision.PoseLandmarker.create_from_options(options)
    print("2. AI 엔진 초기화 성공!")
except Exception as e:
    print(f"❌ 엔진 초기화 실패: {e}")
    sys.exit()

cap = cv.VideoCapture(0)
print(f"3. 웹캠 작동 중... (유니티 포트 {UDP_PORT}로 송신)")

# [중요] try-finally를 사용하여 프로그램 종료 시 자원 해제 보장
try:
    while cap.isOpened():
        success, image = cap.read()
        if not success: continue

        image_rgb = cv.cvtColor(image, cv.COLOR_BGR2RGB)
        mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=image_rgb)
        detection_result = detector.detect(mp_image)

        if detection_result.pose_landmarks:
            for landmark_list in detection_result.pose_landmarks:
                # 좌표 변환 (y값 반전)
                coords = [f"{lm.x},{1 - lm.y},{lm.z}" for lm in landmark_list]
                data_string = ",".join(coords)
                
                sock.sendto(data_string.encode(), (UDP_IP, UDP_PORT))
                
                # 시각화 (cv 사용)
                for lm in landmark_list:
                    h, w, _ = image.shape
                    cv.circle(image, (int(lm.x * w), int(lm.y * h)), 5, (0, 255, 0), -1)
            
            print(f"유니티로 데이터 송신 중...", end="\r")

        cv.imshow('Wonhee M4 Mocap -> Unity', image)
        if cv.waitKey(5) & 0xFF == ord('q'): break

finally:
    # [핵심] 루프가 끝나면 무조건 실행되는 종료 구간
    cap.release()
    cv.destroyAllWindows()
    sock.close() # 소켓을 닫아 포트 점유 해제
    print("\n파이썬 서버 및 네트워크가 안전하게 종료되었습니다.")