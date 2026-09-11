# KIMM Car Simulator

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6%20(6000.3.10f1)%20URP-black?style=for-the-badge&logo=unity" />
  <img src="https://img.shields.io/badge/ROS_2-Humble%20%7C%20Foxy-blue?style=for-the-badge&logo=ros" />
  <img src="https://img.shields.io/badge/Dynamics-14--DOF%20FMU%20(1000Hz)-brightgreen?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Platform-Windows%20x64%20%7C%20Linux-orange?style=for-the-badge" />
</p>

<p align="center">
  <b>14자유도(14-DOF) 다물체 동역학 FMU 및 Unity 3D, ROS 2 기반 실시간 자율주행 차량 디지털 트윈 시뮬레이터</b><br>
  <i>An Open-Source Real-Time Autonomous Driving Digital Twin Simulator Framework Based on Simscape Multibody 14-DOF FMU, Unity 3D, and ROS 2.</i>
</p>

---

## 📑 목차 (Table of Contents)

- [개요 (Overview)](#-개요-overview)
- [주요 기능 (Key Features)](#-주요-기능-key-features)
- [시작하기 및 배포본 실행 (Getting Started)](#-시작하기-및-배포본-실행-getting-started)
  - [1. Windows 환경 실행](#1-windows-환경-실행)
  - [2. Linux 환경 실행](#2-linux-환경-실행)
- [ROS 2 연동 및 통신 명세 (ROS 2 Integration & Topics)](#-ros-2-연동-및-통신-명세-ros-2-integration--topics)
  - [1. ROS-TCP-Endpoint 설치 및 빌드](#1-ros-tcp-endpoint-설치-및-빌드)
  - [2. KIMM 제어 메시지 패키지 (`kimm_msgs`) 다운로드 및 빌드](#2-kimm-제어-메시지-패키지-kimm_msgs-다운로드-및-빌드)
  - [3. ROS-TCP-Endpoint 서버 실행](#3-ros-tcp-endpoint-서버-실행)
  - [4. KIMM 차량 제어 메시지 명세 (`kimm_msgs/CarControlCmd`)](#4-kimm-차량-제어-메시지-명세-kimm_msgscarcontrolcmd)
  - [5. ROS 2 토픽 인터페이스 전체 명세 (Topics Specification)](#5-ros-2-토픽-인터페이스-전체-명세-topics-specification)
  - [6. 자율주행 경로 추종 제어 Python 예제 코드 (`pure_pursuit_demo.py`)](#6-자율주행-경로-추종-제어-python-예제-코드-pure_pursuit_demopy)
- [조작 및 단축키 안내 (Controls Guide)](#-조작-및-단축키-안내-controls-guide)
- [런타임 환경설정 (Configuration & Hot-Swap)](#-런타임-환경설정-configuration--hot-swap)
  - [1. 차량 물리 파라미터 기본 설정 (`vehicle_config.json` - 42개 동역학 변수)](#1-차량-물리-파라미터-기본-설정-vehicle_configjson---42개-동역학-변수)
  - [2. 센서 스위트 기본 설정 (`default_sensor_config.json`)](#2-센서-스위트-기본-설정-default_sensor_configjson)
  - [3. 다중 센서 확장 설정 예시 (`custom_sensor_config.json`)](#3-다중-센서-확장-설정-예시-custom_sensor_configjson)
- [지원 맵 및 트랙 (Supported Tracks)](#-지원-맵-및-트랙-supported-tracks)

---

## 🌟 개요 (Overview)

**KIMM Car Simulator**는 중소·중견 자동차 부품사, 스타트업 및 연구기관의 고가 외산 상용 시뮬레이터 도입 비용 부담을 덜고, 실차 수준의 고충실도 가상 검증 환경을 제공하기 위해 개발된 **오픈소스 자율주행 차량 디지털 트윈 시뮬레이터**이다.

> 📖 **[공식 사용자 매뉴얼 PDF 열람 및 다운로드 (User Manual PDF)](docs/KIMM_Car_Simulator_User_Manual.pdf)**  
> 시뮬레이터 UI 가이드, 키보드/레이싱휠 조작법, 시나리오 편집 모드 등 상세 설명이 포함된 공식 매뉴얼 문서

* **1000 Hz 다물체 동역학**: MATLAB Simscape Multibody 기반 14자유도(14-DOF) 모델과 Magic Formula 비선형 타이어 마찰 모델을 C++ FMU로 연동하여 1ms Co-Simulation을 수행한다.
* **센서 및 파라미터 핫스왑**: 소스코드 재컴파일 없이 JSON 파일만으로 42개 차량 동역학 물리 변수와 복수 LiDAR/Camera/GNSS/IMU 구성을 실시간 교체한다.
* **표준 ROS 2 인터페이스**: 실제 차량 By-Wire 제어 규격(`kimm_msgs/CarControlCmd`)을 통해 Ubuntu ROS 2 자율주행 풀스택(인지-판단-제어)과 1:1 직통 통신을 지원한다.

---

## ✨ 주요 기능 (Key Features)

| 분류 | 주요 기능 및 기술 스펙 |
| :--- | :--- |
| **🚗 차량 동역학** | • **14자유도 다물체 모델**: 차체 6-DOF, 4륜 독립 서스펜션 4-DOF, 휠 회전 4-DOF<br>• **Magic Formula 타이어 모델**: 4바퀴 실시간 노면 고도/경사각 레이캐스트 및 비선형 구동/제동/코너링 포스 계산<br>• **1000 Hz Co-Simulation**: FMI 2.0 표준 C++ 바이너리 기반 1ms 결정론적 연성 해석 |
| **📡 센서 스위트** | • **3D LiDAR**: 16/32/64채널 병렬 레이캐스트 및 `sensor_msgs/PointCloud2` 발행<br>• **HD Camera**: RGB 전/후방/어라운드뷰 렌더 텍스처 및 `sensor_msgs/CompressedImage` 스트리밍<br>• **GNSS & IMU**: WGS84 위경도 변환(`NavSatFix`) 및 100Hz 6축 관성 센서 데이터(`Imu`) |
| **🌐 통신 및 연동** | • **ROS 2 By-Wire 인터페이스**: `kimm_msgs/CarControlCmd` (가속, 제동, 조향, 기어)<br>• **비동기 TCP 게이트웨이**: `ROS-TCP-Endpoint` 기반 다중 스레드 고속 통신 |
| **🛠️ 사용자 편의 기능** | • **실시간 텔레메트리 차트 (`TAB`)**: 속도, 롤/피치각, 서스펜션 변위, 슬립률 동적 시각화<br>• **인터랙티브 시나리오 편집 모드 (`E`)**: 주행 정지 후 과속방지턱, 드럼통, 더미차량, 동적 보행자 실시간 배치<br>• **ESC 시스템 메뉴**: 맵 전환, 파일 탐색기 기반 차량/센서 JSON 즉시 핫스왑 |

---

## 🚀 시작하기 및 배포본 실행 (Getting Started)

시뮬레이터는 별도의 소스코드 빌드 과정 없이, 사전에 빌드된 독립 실행 패키지를 다운로드하여 즉시 구동할 수 있다.

### 1. Windows 환경 실행
1. 저장소 우측의 **[Releases](https://github.com/dbsrn0125/Kimm-Car-Simulator/releases)** 페이지에서 최신 Windows 패키지(`Kimm-Car-Simulator_v1.0.0_Windows_x64.zip`)를 다운로드한다.
2. 다운로드한 `.zip` 압축파일을 원하는 디렉토리에 해제한다.
3. 폴더 내의 **`Kimm-Car-Simulator.exe`** 를 더블클릭하여 실행한다.
4. 키보드(`W/A/S/D`) 또는 USB 레이싱 휠을 사용하여 주행을 시작한다.

### 2. Linux 환경 실행
1. 저장소 우측의 **[Releases](https://github.com/dbsrn0125/Kimm-Car-Simulator/releases)** 페이지에서 최신 Linux 패키지(`Kimm-Car-Simulator_v1.0.0_Linux_x64.tar.gz`)를 다운로드한다.
2. 터미널을 열고 압축을 해제한 후 실행 권한을 부여한다:
   ```bash
   tar -zxvf Kimm-Car-Simulator_v1.0.0_Linux_x64.tar.gz
   cd Kimm-Car-Simulator_Linux
   chmod +x Kimm-Car-Simulator.x86_64
   ./Kimm-Car-Simulator.x86_64
   ```

---

## 🌐 ROS 2 연동 및 통신 명세 (ROS 2 Integration & Topics)

Ubuntu 22.04 (Humble / Foxy) 또는 Windows WSL2 환경에서 시뮬레이터와 TCP 소켓으로 고속 연동하여 센서 데이터 수신 및 By-Wire 자율주행 제어를 수행할 수 있다.

### 1. ROS-TCP-Endpoint 설치 및 빌드
Unity와 ROS 2 간의 비동기 TCP 통신을 지원하는 공식 라이브러리인 [Unity-Technologies/ROS-TCP-Endpoint](https://github.com/Unity-Technologies/ROS-TCP-Endpoint)를 워크스페이스에 클론하고 빌드한다:

```bash
# 1. ROS 2 워크스페이스 생성 및 이동
mkdir -p ~/ros2_ws/src
cd ~/ros2_ws/src

# 2. Unity 공식 ROS-TCP-Endpoint 저장소 클론
git clone -b main-ros2 https://github.com/Unity-Technologies/ROS-TCP-Endpoint.git

# 3. 패키지 빌드
cd ~/ros2_ws
colcon build --packages-select ros_tcp_endpoint
source install/setup.bash
```

---

### 2. KIMM 제어 메시지 패키지 (`kimm_msgs`) 다운로드 및 빌드
차량 By-Wire 제어 명령(`/kimm/car_cmd`)을 발행하기 위한 커스텀 메시지 패키지(`kimm_msgs`)를 다운로드하여 빌드한다:

```bash
cd ~/ros2_ws/src

# 1. KIMM ROS 2 메시지 패키지 다운로드 및 압축 해제 (1초 컷)
wget https://github.com/dbsrn0125/Kimm-Car-Simulator/releases/download/v1.0.0/kimm_ros2_packages.zip
unzip kimm_ros2_packages.zip

# 2. kimm_msgs 메시지 패키지 빌드 및 환경 적용
cd ~/ros2_ws
colcon build --packages-select kimm_msgs
source install/setup.bash
```

---

### 3. ROS-TCP-Endpoint 서버 실행
```bash
# ROS 2 환경 활성화
source ~/ros2_ws/install/setup.bash

# TCP 엔드포인트 서버 실행 (기본 포트: 10000)
ros2 run ros_tcp_endpoint default_server_endpoint --ros-args -p ROS_IP:=0.0.0.0 -p ROS_PORT:=10000
```
> 시뮬레이터 실행 후 화면 상단 HUD의 ROS 2 상태 표시기가 **초록색 (Connected)** 으로 점등되면 정상 연동된 상태이다.

---

### 4. KIMM 차량 제어 메시지 명세 (`kimm_msgs/CarControlCmd`)
외부 자율주행 알고리즘 노드가 시뮬레이터 차량을 By-Wire 방식으로 전자 제어하기 위해 `/kimm/car_cmd` 토픽으로 발행하는 메시지 정의이다:

```yaml
# kimm_msgs/msg/CarControlCmd.msg
float32 accel      # 가속 페달 답력 (0.0 ~ 1.0)
float32 brake      # 유압 제동 페달 답력 (0.0 ~ 1.0)
float32 steering   # 전륜 조향각 (-1.0: 최대 좌회전, +1.0: 최대 우회전)
int32 gear         # 전자식 변속 기어 (0: Park, 1: Drive, -1: Reverse)
```

---

### 5. ROS 2 토픽 인터페이스 전체 명세 (Topics Specification)

기본 센서 구성(`default_sensor_config.json`) 기준 발행/구독 토픽 목록:

| 토픽명 (Topic Name) | 메시지 타입 (Type) | 주파수 (Hz) | 설명 |
| :--- | :--- | :--- | :--- |
| **`/kimm/vehicle_status`** | `nav_msgs/msg/Odometry` | 50.0 | 실시간 차속, 전역 오도메트리 위치$(X, Y, Z)$, 쿼터니언 헤딩 자세 |
| **`/kimm/lidar/points`** | `sensor_msgs/msg/PointCloud2` | 20.0 | 3D LiDAR 포인트클라우드 (100,000 pts/scan) |
| **`/kimm/camera/color/compressed`** | `sensor_msgs/msg/CompressedImage` | 30.0 | 전방 HD RGB 카메라 JPEG 실시간 영상 스트림 |
| **`/kimm/gnss/fix`** | `sensor_msgs/msg/NavSatFix` | 10.0 | WGS84 위도, 경도, 타원체 고도 데이터 |
| **`/kimm/imu/data`** | `sensor_msgs/msg/Imu` | 100.0 | 6축 관성 센서 각속도 및 3축 가속도 데이터 |
| **`/kimm/goal_pose`** | `geometry_msgs/msg/PoseStamped` | Event | Auto Mode 화면 핀(Pin 📍) 클릭 시 5회 연속 브로드캐스트 발행 |
| **`/kimm/car_cmd`** | `kimm_msgs/msg/CarControlCmd` | 100.0 | 외부 자율주행 제어기로부터의 By-Wire 주행 명령 (수신) |

---

### 6. 자율주행 경로 추종 제어 Python 예제 코드 (`pure_pursuit_demo.py`)

시뮬레이터 차량을 자율주행 제어 모드(`AUTO MODE`, 단축키 `M`)로 전환하면 **화면에서 마우스 좌클릭으로 바닥에 목표 핀(Goal Pin 📍)을 직접 배치**할 수 있다. 핀이 배치되면 시뮬레이터가 `/kimm/goal_pose` 토픽을 5회 연속 전송하며, 아래 파이썬 스크립트가 이를 수신하여 **Pure Pursuit 조향각 산출, 거리 기반 적응형 감속, 2.0m 이내 정밀 정차**를 수행한다:

```python
#!/usr/bin/env python3
import rclpy
from rclpy.node import Node
import math
from nav_msgs.msg import Odometry
from geometry_msgs.msg import PoseStamped
from kimm_msgs.msg import CarControlCmd

class KimmPurePursuitController(Node):
    def __init__(self):
        super().__init__('kimm_pure_pursuit_controller')
        # 시뮬레이터 차량 상태(Odometry) 및 마우스 핀 목표 지점(Goal Pose) 구독
        self.sub_odom = self.create_subscription(Odometry, '/kimm/vehicle_status', self.odom_cb, 10)
        self.sub_goal = self.create_subscription(PoseStamped, '/kimm/goal_pose', self.goal_cb, 10)
        # By-Wire 제어 명령 퍼블리셔 생성
        self.pub_cmd = self.create_publisher(CarControlCmd, '/kimm/car_cmd', 10)
        
        self.current_x = None
        self.current_y = None
        self.current_speed = 0.0
        self.current_yaw = 0.0
        self.goal_x = None
        self.goal_y = None
        
        self.timer = self.create_timer(0.05, self.control_loop) # 20 Hz 제어 루프
        self.get_logger().info("🚀 [KIMM Pure Pursuit Node] Autonomous Controller Ready!")

    def odom_cb(self, msg):
        self.current_x = msg.pose.pose.position.x
        self.current_y = msg.pose.pose.position.y
        
        vx = msg.twist.twist.linear.x
        vy = msg.twist.twist.linear.y
        self.current_speed = math.hypot(vx, vy)

        q = msg.pose.pose.orientation
        siny_cosp = 2.0 * (q.w * q.z + q.x * q.y)
        cosy_cosp = 1.0 - 2.0 * (q.y * q.y + q.z * q.z)
        self.current_yaw = math.atan2(siny_cosp, cosy_cosp)

    def goal_cb(self, msg):
        self.goal_x = msg.pose.position.x
        self.goal_y = msg.pose.position.y
        self.get_logger().info(f"🚩 New Goal Pin Received: X={self.goal_x:.2f}, Y={self.goal_y:.2f}")

    def control_loop(self):
        if self.current_x is None or self.goal_x is None:
            return

        dx = self.goal_x - self.current_x
        dy = self.goal_y - self.current_y
        dist = math.hypot(dx, dy)

        cmd = CarControlCmd()
        
        # 1. 목표 지점 2.0m 이내 정밀 정차
        if dist < 2.0:
            cmd.accel = 0.0
            cmd.brake = 1.0
            cmd.steering = 0.0
            cmd.gear = 1
            self.get_logger().info("🎯 Goal Destination Reached! Vehicle Safely Stopped.")
            self.goal_x = None
            self.goal_y = None
        else:
            # 2. Pure Pursuit 조향각 산출
            target_angle = math.atan2(dy, dx)
            alpha = target_angle - self.current_yaw
            
            while alpha > math.pi: alpha -= 2 * math.pi
            while alpha < -math.pi: alpha += 2 * math.pi

            steering_cmd = math.sin(alpha) * 1.5
            cmd.steering = max(-1.0, min(1.0, steering_cmd))

            # 3. 거리 기반 적응형 감속 프로파일링 (25m 전부터 감속)
            if dist < 25.0:
                target_speed_mps = max(0.6, dist * 0.2)
                if self.current_speed > target_speed_mps:
                    cmd.accel = 0.0
                    cmd.brake = min(1.0, (self.current_speed - target_speed_mps) * 0.5 + 0.15)
                else:
                    cmd.accel = 0.08
                    cmd.brake = 0.0
            else:
                cmd.accel = 0.4
                cmd.brake = 0.0

            cmd.gear = 1

        self.pub_cmd.publish(cmd)

def main(args=None):
    rclpy.init(args=args)
    node = KimmPurePursuitController()
    try:
        rclpy.spin(node)
    except KeyboardInterrupt:
        pass
    finally:
        node.destroy_node()
        rclpy.shutdown()

if __name__ == '__main__':
    main()
```

#### 📍 인터랙티브 마우스 목표 핀(Goal Pin) 지정 방법
1. 시뮬레이터 주행 중 키보드 **`M` 키**를 눌러 **`AUTO MODE`** 로 전환한다.
2. 마우스 커서가 활성화되면 가상 트랙 도로 바닥의 원하는 위치를 **마우스 좌클릭**한다.
3. 3D 지형에 붉은색 **목표 핀(Goal Pin 📍)** 이 꽂히며, 시뮬레이터는 패킷 유실 방지를 위해 ROS 2 **`/kimm/goal_pose` 토픽을 5회 연속 브로드캐스트**한다.
4. 실행 중인 `pure_pursuit_demo.py`가 이를 수신하여 차량이 즉시 해당 목표 핀을 향해 자율주행을 시작한다.

---

## 🎮 조작 및 단축키 안내 (Controls Guide)

### 1. 키보드 조작 (Keyboard Controls)

| 키 (Key) | 기능 (Function) | 상세 설명 |
| :---: | :--- | :--- |
| **`W` / `S`** | **가속 / 감속 (Throttle / Brake)** | 차량 종방향 가속 및 유압 제동 |
| **`A` / `D`** | **조향 (Steering Left / Right)** | 전륜 조향각 제어 |
| **`Shift` / `Ctrl`** | **기어 전진 / 후진 (Drive / Reverse)** | `Shift`: 전진(D) 기어 체결 / `Ctrl`: 후진(R) 기어 체결 |
| **`P`** | **파킹 기어 (Park)** | 주차(P) 기어 체결 (`gear: 0`) |
| **`M`** | **수동 / 자율 모드 전환 (Manual ↔ Auto)** | 운전자 수동 주행과 ROS 2 자율주행 제어권 1:1 토글 |
| **`R`** | **차량 위치 리셋 (Reset Vehicle)** | 현재 맵의 최초 시작 스폰 위치로 즉시 재배치 |
| **`E`** | **인터랙티브 시나리오 편집 모드 토글** | 주행 정지 후 장애물/보행자 마우스 배치 모드 진입 |
| **`TAB`** | **실시간 텔레메트리 차트 토글** | 속도, 롤/피치, 서스펜션 변위, 슬립률 그래프 오버레이 |
| **`ESC`** | **메인 시스템 메뉴 열기 / 닫기** | 맵 전환, 차량/센서 JSON 핫스왑, 설정 팝업 |
| **`F1`** | **조작 도움말 팝업 토글** | 전체 키 매핑 및 휠 조작 안내창 표시 |
| **`F5` ~ `F8`** | **시점 전환 (Camera Views)** | `F5`: 1인칭/3인칭, `F6`: 탑뷰, `F7`/`F8`: 좌/우 사이드뷰 |

### 2. USB 레이싱 휠 & 게임패드 매핑 (Racing Wheel Mapping)

* **스티어링 휠 / 페달**: 아날로그 조향각 및 가속(Gas) / 제동(Brake) 페달 답력 비례 제어 (0 ~ 100%)
* **패들 시프트**: 우측 패들(D 기어 체결) / 좌측 패들(R 기어 $\leftrightarrow$ P 기어 순환)
* **버튼 매핑**:
  * 🔵 **`A` 버튼 (South)**: 수동 ↔ 자율주행 모드 전환 (`M` 키 동일)
  * 🔴 **`B` 버튼 (East)**: 차량 위치 리셋 (`R` 키 동일)
  * 🟢 **`X` 버튼 (West)**: HUD 미니 뷰포트 시점 변경
  * 🟡 **`Y` 버튼 (North)**: 맵 순환 로딩 (`K-City ➔ Mcity ➔ ZalaZone ➔ PG`)
  * **십자키 (D-Pad)**: 상/하/좌/우 카메라 뷰 즉시 전환

---

## ⚙️ 런타임 환경설정 (Configuration & Hot-Swap)

설정 파일은 실행 파일 기준 `Kimm-Car-Simulator_Data/StreamingAssets/` 폴더에 위치하며, 소스코드 수정 없이 JSON 수정만으로 동작이 변경된다.

### 1. 차량 물리 파라미터 기본 설정 (`vehicle_config.json` - 42개 동역학 변수)

```json
{
  "Metadata": {
    "VehicleName": "KIMM Standard Sedan (Original Baseline)",
    "Description": "인스펙터 원본 데이터 기반 42개 정확한 baseline 파라미터 설정 파일",
    "Version": "1.0"
  },
  "Parameters": {
    "Veh_AeroArea": 2.594,
    "Veh_AeroCd": 0.2888,
    "Veh_AeroCl": 0.149,
    "Veh_AeroRho": 1.205,

    "Veh_BodyInertia[1,1]": 600.0,
    "Veh_BodyInertia[1,2]": 3000.0,
    "Veh_BodyInertia[1,3]": 3200.0,

    "Veh_BodyMass": 1600.0,
    "Veh_BodyRefZ0": 0.6147,
    "Veh_BodytoWheelCenter": 0.2647,
    "Veh_FrontAxleX": 1.5,
    "Veh_RearAxleX": -1.5,
    "Veh_SteerRatio": 16.0,

    "Veh_SuspF_BumpC": 3000.0,
    "Veh_SuspF_BumpK": 200000.0,
    "Veh_SuspF_BumpLimit": -0.084,
    "Veh_SuspF_BumpWidth": 0.003,
    "Veh_SuspF_C": 1750.0,
    "Veh_SuspF_EqPos": 0.1126,
    "Veh_SuspF_K": 35000.0,
    "Veh_SuspF_ReboundC": 3000.0,
    "Veh_SuspF_ReboundK": 200000.0,
    "Veh_SuspF_ReboundLimit": 0.056,
    "Veh_SuspF_ReboundWidth": 0.003,
    "Veh_SuspF_UnsprungInertia[1,1]": 1.0,
    "Veh_SuspF_UnsprungInertia[1,2]": 1.0,
    "Veh_SuspF_UnsprungInertia[1,3]": 1.0,
    "Veh_SuspF_UnsprungMass": 48.0,

    "Veh_SuspR_BumpC": 4000.0,
    "Veh_SuspR_BumpK": 300000.0,
    "Veh_SuspR_BumpLimit": -0.095,
    "Veh_SuspR_BumpWidth": 0.003,
    "Veh_SuspR_C": 2200.0,
    "Veh_SuspR_EqPos": 0.0985,
    "Veh_SuspR_K": 40000.0,
    "Veh_SuspR_ReboundC": 4000.0,
    "Veh_SuspR_ReboundK": 300000.0,
    "Veh_SuspR_ReboundLimit": 0.04,
    "Veh_SuspR_ReboundWidth": 0.003,
    "Veh_SuspR_UnsprungInertia[1,1]": 1.0,
    "Veh_SuspR_UnsprungInertia[1,2]": 1.0,
    "Veh_SuspR_UnsprungInertia[1,3]": 1.0,
    "Veh_SuspR_UnsprungMass": 45.0,

    "Veh_TrackF": 1.6,
    "Veh_TrackR": 1.6
  }
}
```

---

### 2. 센서 스위트 기본 설정 (`default_sensor_config.json`)

단일 센서 구성 기준 기본 설정 파일:

```json
{
  "SensorConfigName": "KIMM Default Sensor Suite",
  "_CoordinateSystem_Guide": {
    "Position": "FLU Standard (x: Forward(+)/Backward(-), y: Left(+)/Right(-), z: Up(+)/Down(-))",
    "Rotation": "Degrees (roll: Right(+), pitch: NoseUp(+)/NoseDown(-), yaw: TurnLeft(+)/TurnRight(-))"
  },
  "ROS2Connection": {
    "RosIP": "127.0.0.1",
    "RosPort": 10000
  },
  "GNSS": {
    "FrameId": "gnss_frame",
    "TopicName": "/kimm/gnss/fix",
    "PublishFrequency": 10.0,
    "InitialLatitude": 0.0,
    "InitialLongitude": 0.0,
    "InitialAltitude": 0.0,
    "LocalPosition": { "x": 0.0, "y": 0.0, "z": 0.0 },
    "LocalRotation": { "roll": 0.0, "pitch": 0.0, "yaw": 0.0 }
  },
  "IMU": {
    "FrameId": "imu_frame",
    "TopicName": "/kimm/imu/data",
    "PublishFrequency": 100.0,
    "LocalPosition": { "x": 0.0, "y": 0.0, "z": 0.0 },
    "LocalRotation": { "roll": 0.0, "pitch": 0.0, "yaw": 0.0 }
  },
  "LiDAR": {
    "FrameId": "lidar_frame",
    "TopicName": "/kimm/lidar/points",
    "PublishFrequency": 20.0,
    "PointsNumPerScan": 100000,
    "MinRange": 0.1,
    "MaxRange": 70.0,
    "GaussianNoiseSigma": 0.02,
    "MaxIntensity": 255.0,
    "LocalPosition": { "x": 0.0, "y": 0.0, "z": 0.6 },
    "LocalRotation": { "roll": 0.0, "pitch": 0.0, "yaw": 0.0 }
  },
  "Camera": {
    "FrameId": "camera_frame",
    "TopicName": "/kimm/camera/color/compressed",
    "CameraInfoTopic": "/kimm/camera/camera_info",
    "PublishFrequency": 30.0,
    "ResolutionWidth": 640,
    "ResolutionHeight": 480,
    "FieldOfView": 60.0,
    "LocalPosition": { "x": 0.3, "y": 0.0, "z": 0.7 },
    "LocalRotation": { "roll": 0.0, "pitch": 0.0, "yaw": 0.0 }
  },
  "VehicleStatus": {
    "TopicName": "/kimm/vehicle_status",
    "PublishFrequency": 50.0
  },
  "CarControlCmd": {
    "TopicName": "/kimm/car_cmd"
  }
}
```

---

### 3. 다중 센서 확장 설정 예시 (`custom_sensor_config.json`)

`"LiDAR"` 또는 `"Camera"` 키 아래에 **배열 `[ { ... }, { ... } ]`** 형태로 작성하면, 전방/후방 라이다 및 다중 카메라가 런타임에 동적으로 각각 인스턴스화된다:

```json
{
  "SensorConfigName": "KIMM Dual-Camera & Dual-LiDAR Suite",
  "ROS2Connection": {
    "RosIP": "127.0.0.1",
    "RosPort": 10000
  },
  "GNSS": {
    "FrameId": "gnss_frame",
    "TopicName": "/kimm/gnss/fix",
    "PublishFrequency": 10.0,
    "InitialLatitude": 36.37561,
    "InitialLongitude": 127.35921,
    "InitialAltitude": 55.0,
    "LocalPosition": { "x": 0.0, "y": 0.0, "z": 0.0 },
    "LocalRotation": { "roll": 0.0, "pitch": 0.0, "yaw": 0.0 }
  },
  "IMU": {
    "FrameId": "imu_frame",
    "TopicName": "/kimm/imu/data",
    "PublishFrequency": 200.0,
    "LocalPosition": { "x": 0.0, "y": 0.0, "z": 0.0 },
    "LocalRotation": { "roll": 0.0, "pitch": 0.0, "yaw": 0.0 }
  },
  "LiDAR": [
    {
      "FrameId": "front_lidar_frame",
      "TopicName": "/kimm/lidar_front/points",
      "PublishFrequency": 10.0,
      "PointsNumPerScan": 50000,
      "MinRange": 0.1,
      "MaxRange": 70.0,
      "GaussianNoiseSigma": 0.02,
      "MaxIntensity": 255.0,
      "LocalPosition": { "x": 0.3, "y": 0.0, "z": 0.6 },
      "LocalRotation": { "roll": 0.0, "pitch": -15.0, "yaw": 0.0 }
    },
    {
      "FrameId": "rear_lidar_frame",
      "TopicName": "/kimm/lidar_rear/points",
      "PublishFrequency": 10.0,
      "PointsNumPerScan": 50000,
      "MinRange": 0.1,
      "MaxRange": 70.0,
      "GaussianNoiseSigma": 0.02,
      "MaxIntensity": 255.0,
      "LocalPosition": { "x": -0.3, "y": 0.0, "z": 0.6 },
      "LocalRotation": { "roll": 0.0, "pitch": -15.0, "yaw": 180.0 }
    }
  ],
  "Camera": [
    {
      "FrameId": "front_camera_frame",
      "TopicName": "/kimm/camera_front/color/compressed",
      "CameraInfoTopic": "/kimm/camera_front/camera_info",
      "PublishFrequency": 30.0,
      "ResolutionWidth": 640,
      "ResolutionHeight": 480,
      "FieldOfView": 60.0,
      "LocalPosition": { "x": 0.3, "y": 0.0, "z": 0.7 },
      "LocalRotation": { "roll": 0.0, "pitch": 0.0, "yaw": 0.0 }
    },
    {
      "FrameId": "rear_camera_frame",
      "TopicName": "/kimm/camera_rear/color/compressed",
      "CameraInfoTopic": "/kimm/camera_rear/camera_info",
      "PublishFrequency": 30.0,
      "ResolutionWidth": 640,
      "ResolutionHeight": 480,
      "FieldOfView": 60.0,
      "LocalPosition": { "x": -0.3, "y": 0.0, "z": 0.7 },
      "LocalRotation": { "roll": 0.0, "pitch": 0.0, "yaw": 180.0 }
    }
  ]
}
```

---

## 🗺️ 지원 맵 및 트랙 (Supported Tracks)

| 맵 명칭 (Track Name) | 특징 및 환경 구성 | 권장 테스트 시나리오 |
| :--- | :--- | :--- |
| **K-City (자율주행 실험도시)** | 한국교통안전공단(KATRI) 실도로 기반 교차로, 건물, 터널 트랙 | 차선 인식, 3D LiDAR 장애물 회피, 도심 신호 교차로 주행 |
| **Mcity (도심 자율주행 트랙)** | 도심형 인터체인지 및 복합 도로 환경 트랙 | 복합 교차로 회전, 다중 보행자/더미차량 돌발 상황 검증 |
| **ZalaZone (자라존 시험장)** | 유럽형 스마트시티 및 고속 핸들링/선회 시험 트랙 | 고속 자율주행 경로 추종, 복합 슬라럼 및 횡방향 핸들링 검증 |
| **Proving Ground (PG)** | 평탄한 다목적 차량 종합 성능 시험장 및 직선/선회로 | 차량 14-DOF 동역학 한계 거동, 급가속/급제동, 서스펜션 튜닝 |
