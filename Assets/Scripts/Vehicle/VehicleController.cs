using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("FMU Manager Reference")]
    public FMUManager fmuManager;
    public VehicleInputManager inputManager;

    [Header("Visual Mesh & Auto-Fitting Settings")]
    [Tooltip("3D 차량 섀시/차체 껍데기 Visual Mesh Transform")]
    public Transform chassisVisualTransform;

    [Tooltip("Default Config 기준 섀시 로컬 오프셋 위치 (X, Y, Z) - 사용자 최적 검증값: (0, 0.15, -0.05)")]
    public Vector3 defaultChassisLocalPos = new Vector3(0.0f, 0.15f, -0.05f);

    [Tooltip("Default Config 기준 섀시 로컬 스케일 (X, Y, Z) - 사용자 최적 검증값: (1, 1, 1.05)")]
    public Vector3 defaultChassisLocalScale = new Vector3(1.0f, 1.0f, 1.05f);

    [Tooltip("Baseline 윤거 (m) - Default vehicle_config.json 기준 (Veh_TrackF = 1.6m)")]
    public float baseTrackWidth = 1.600f;

    [Tooltip("Baseline 축거 (m) - Default vehicle_config.json 기준 (FrontAxleX 1.5m + RearAxleX 1.5m = 3.0m)")]
    public float baseWheelbase = 3.000f;

    // 절대 변하지 않는 100% 퓨어 원본 스케일 및 위치
    private Vector3 _pureOriginalScale = Vector3.one;
    private Vector3 _pureOriginalLocalPos = Vector3.zero;
    private bool _isOriginalCached = false;

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    private Vector3 _spawnPos;
    private Quaternion _spawnRot;

    [Header("FMU Chassis Variable Names")]
    public string var_Steer_In = "str_angle";
    public string var_Throttle_In = "accel";
    public string var_Brake_In = "brake";
    public string var_Gear_In = "gear";

    public string out_ChassisPos_X = "Veh_BodyPos_X";
    public string out_ChassisPos_Y = "Veh_BodyPos_Y";
    public string out_ChassisPos_Z = "Veh_BodyPos_Z";
    public string out_ChassisRot_X = "Veh_BodyRot_X";
    public string out_ChassisRot_Y = "Veh_BodyRot_Y";
    public string out_ChassisRot_Z = "Veh_BodyRot_Z";
    public string out_ChassisRot_W = "Veh_BodyRot_W";

    public string out_Toe_Left = "Veh_Steer_L";
    public string out_Toe_Right = "Veh_Steer_R";

    [Header("Wheels Configuration")]
    public List<WheelData> wheels = new List<WheelData>();

    private bool _isRespawning = false;

    private void Awake()
    {
        if (FindFirstObjectByType<SensorConfigManager>() == null) gameObject.AddComponent<SensorConfigManager>();
        if (fmuManager == null) fmuManager = FindFirstObjectByType<FMUManager>();
        if (inputManager == null) inputManager = FindFirstObjectByType<FMUManager>()?.GetComponent<VehicleInputManager>();

        CachePureOriginalTransform();
    }

    private void Start()
    {
        CachePureOriginalTransform();
        AttachSpoilerToChassis();

        // 초기 시작 시 기본 최적 피팅 적용
        ApplyChassisScale(baseTrackWidth, baseWheelbase);

        if (spawnPoint != null)
        {
            ResetVehicle(spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            ResetVehicle(transform.position, transform.rotation);
        }

        if (inputManager != null)
        {
            inputManager.OnResetTriggered += () =>
            {
                if (spawnPoint != null)
                {
                    ResetVehicle(spawnPoint.position, spawnPoint.rotation);
                }
                else
                {
                    ResetVehicle(Vector3.zero, Quaternion.identity);
                }
            };
        }
    }

    private void CachePureOriginalTransform()
    {
        if (_isOriginalCached) return;

        if (chassisVisualTransform == null)
        {
            Transform visualChild = transform.Find("Chassis") ?? transform.Find("Visual") ?? transform.Find("Body");
            if (visualChild != null)
            {
                chassisVisualTransform = visualChild;
            }
            else
            {
                MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
                if (mr != null) chassisVisualTransform = mr.transform;
            }
        }

        if (chassisVisualTransform != null)
        {
            _pureOriginalScale = chassisVisualTransform.localScale;
            _pureOriginalLocalPos = chassisVisualTransform.localPosition;
            _isOriginalCached = true;
        }
    }

    private void AttachSpoilerToChassis()
    {
        if (chassisVisualTransform == null) return;

        Transform[] allTransforms = GetComponentsInChildren<Transform>(true);
        foreach (Transform tf in allTransforms)
        {
            if (tf == null || tf == chassisVisualTransform) continue;

            string nameLower = tf.name.ToLower();
            if (nameLower.Contains("spoiler") || nameLower.Contains("wing") || nameLower.Contains("rearwing"))
            {
                if (!tf.IsChildOf(chassisVisualTransform))
                {
                    tf.SetParent(chassisVisualTransform, true);
                }
            }
        }
    }

    /// <summary>
    /// [차량 섀시 3D 피팅 알고리즘]
    /// 런타임 JSON 핫스왑 시 로드된 윤거(currentTrackW)와 축거(currentWheelbase) 수치에 비례하여
    /// 3D 섀시 메쉬의 로컬 위치(오프셋) 및 스케일을 정밀하게 자동 변환한다.
    ///
    /// [원리 수식]:
    /// - Scale_X = defaultScale.x * (currentTrackW / baseTrackWidth)
    /// - Scale_Y = defaultScale.y (서스펜션 차고 높이 유지)
    /// - Scale_Z = defaultScale.z * (currentWheelbase / baseWheelbase)
    /// - Pos_Z   = defaultPos.z   * (currentWheelbase / baseWheelbase) (휠하우스 중심점 보정)
    /// </summary>
    public void ApplyChassisScale(float currentTrackW, float currentWheelbase)
    {
        CachePureOriginalTransform();
        if (chassisVisualTransform == null) return;
        if (baseTrackWidth <= 0 || baseWheelbase <= 0) return;

        // 윤거 및 축거 변화 비율 계산
        float scaleXRatio = currentTrackW / baseTrackWidth;
        float scaleZRatio = currentWheelbase / baseWheelbase;

        // 1. 3D 차체 스케일 인가 (기본 검증 스케일 Vector3(1.0, 1.0, 1.05) 기준 비례 확대/축소)
        Vector3 autoCalculatedScale = new Vector3(
            defaultChassisLocalScale.x * scaleXRatio,
            defaultChassisLocalScale.y,
            defaultChassisLocalScale.z * scaleZRatio
        );
        chassisVisualTransform.localScale = autoCalculatedScale;

        // 2. 3D 차체 로컬 위치 오프셋 인가 (기본 검증 위치 Vector3(0, 0.15, -0.05) 기준 휠하우스 센터 보정)
        Vector3 autoCalculatedPos = new Vector3(
            defaultChassisLocalPos.x * scaleXRatio,
            defaultChassisLocalPos.y,
            defaultChassisLocalPos.z * scaleZRatio
        );
        chassisVisualTransform.localPosition = autoCalculatedPos;
    }

    private void FixedUpdate()
    {
        if (fmuManager == null || inputManager == null) return;

        float targetSteer = inputManager.Steering * (450.0f * Mathf.Deg2Rad);
        if (!string.IsNullOrEmpty(var_Steer_In)) fmuManager.SetValue(var_Steer_In, targetSteer);
        if (!string.IsNullOrEmpty(var_Throttle_In)) fmuManager.SetValue(var_Throttle_In, inputManager.Accel);
        if (!string.IsNullOrEmpty(var_Brake_In)) fmuManager.SetValue(var_Brake_In, inputManager.Brake);
        if (!string.IsNullOrEmpty(var_Gear_In)) fmuManager.SetValue(var_Gear_In, (int)inputManager.Gear);

        foreach (var w in wheels)
        {
            if (w.sensor != null) w.sensor.CalculateGroundForces();

            if (!string.IsNullOrEmpty(w.var_GroundDist_In))
            {
                float relativeGroundY = w.sensor.isGrounded ? (w.sensor.hitPointY - _spawnPos.y) : 0f;
                fmuManager.SetValue(w.var_GroundDist_In, relativeGroundY);
            }

            // 지면 경사각 주입 (FMU 모델 규격: qy에 오르막 음수 Pitch, qx에 우측 뱅크 양수 Roll 매핑)
            if (!string.IsNullOrEmpty(w.var_GroundQx_In)) // 인스펙터에 FL_qy(Pitch)로 매핑됨
            {
                float pitchVal = w.sensor.isGrounded ? w.sensor.hitQx : 0f;
                fmuManager.SetValue(w.var_GroundQx_In, pitchVal);
            }

            if (!string.IsNullOrEmpty(w.var_GroundQy_In)) // 인스펙터에 FL_qx(Roll)로 매핑됨
            {
                float rollVal = w.sensor.isGrounded ? w.sensor.hitQy : 0f;
                fmuManager.SetValue(w.var_GroundQy_In, rollVal);
            }
        }

        fmuManager.DoStep();

        ApplyFMUState();
        ApplyWheels();
    }

    private void ApplyFMUState()
    {
        if (fmuManager == null || !fmuManager.IsFMUActive()) return;

        float cx = (float)fmuManager.GetValue(out_ChassisPos_X);
        float cy = (float)fmuManager.GetValue(out_ChassisPos_Y);
        float cz = (float)fmuManager.GetValue(out_ChassisPos_Z);
        Vector3 fmuPos = new Vector3(cx, cy, cz);

        float qx = (float)fmuManager.GetValue(out_ChassisRot_X);
        float qy = (float)fmuManager.GetValue(out_ChassisRot_Y);
        float qz = (float)fmuManager.GetValue(out_ChassisRot_Z);
        float qw = (float)fmuManager.GetValue(out_ChassisRot_W);

        Quaternion fmuRot = new Quaternion(qx, qy, qz, qw);

        transform.position = _spawnPos + (_spawnRot * fmuPos);
        transform.rotation = _spawnRot * fmuRot;
    }

    private void ApplyWheels()
    {
        if (fmuManager == null || !fmuManager.IsFMUActive()) return;
        float toeL = (float)fmuManager.GetValue(out_Toe_Left);
        float toeR = (float)fmuManager.GetValue(out_Toe_Right);

        foreach (var w in wheels)
        {
            if (!string.IsNullOrEmpty(w.var_WheelPos_X))
            {
                float wx = (float)fmuManager.GetValue(w.var_WheelPos_X);
                float wy = (float)fmuManager.GetValue(w.var_WheelPos_Y);
                float wz = (float)fmuManager.GetValue(w.var_WheelPos_Z);
                w.wheelRoot.localPosition = new Vector3(wx, wy, wz);
            }

            if (!string.IsNullOrEmpty(w.var_WheelRot_X))
            {
                float rx = (float)fmuManager.GetValue(w.var_WheelRot_X);
                float ry = (float)fmuManager.GetValue(w.var_WheelRot_Y);
                float rz = (float)fmuManager.GetValue(w.var_WheelRot_Z);
                float rw = (float)fmuManager.GetValue(w.var_WheelRot_W);
                w.wheelRoot.localRotation = new Quaternion(rx, ry, rz, rw);
            }
            else if (w.id == "FL" || w.id == "FR")
            {
                Quaternion toeRot = Quaternion.identity;
                if (w.id == "FL") toeRot = Quaternion.Euler(0, toeL * Mathf.Rad2Deg, 0);
                else if (w.id == "FR") toeRot = Quaternion.Euler(0, toeR * Mathf.Rad2Deg, 0);
                w.wheelRoot.localRotation = toeRot;
            }

            if (!string.IsNullOrEmpty(w.var_WheelSpin_Out))
            {
                float spin = (float)fmuManager.GetValue(w.var_WheelSpin_Out);
                w.wheelVisual.localRotation = Quaternion.Euler(spin * Mathf.Rad2Deg, 0, 0);
            }
        }
    }

    public void ResetVehicle(Vector3 targetAnchorPos, Quaternion targetAnchorRot)
    {
        _spawnPos = targetAnchorPos;
        _spawnRot = targetAnchorRot;

        if (fmuManager != null)
        {
            fmuManager.ResetFMU();
        }

        ChartModule[] activeCharts = FindObjectsByType<ChartModule>(FindObjectsSortMode.None);
        foreach (ChartModule chart in activeCharts)
        {
            if (chart != null) chart.ResetChartData();
        }

        transform.position = _spawnPos;
        transform.rotation = _spawnRot;

        UnitySensors.Sensor.IMU.KimmIMUSensor kimmIMU = GetComponentInChildren<UnitySensors.Sensor.IMU.KimmIMUSensor>();
        if (kimmIMU != null) kimmIMU.CaptureSpawnHeading();

        if (KimmGoalPosePublisher.Instance != null)
        {
            KimmGoalPosePublisher.Instance.ResetGoalPose();
        }
    }

    private IEnumerator CollisionRespawnRoutine()
    {
        _isRespawning = true;
        if (inputManager != null) inputManager.SetInputActive(false);
        Vector3 forwardVec = transform.forward;
        forwardVec.y = 0;
        forwardVec.Normalize();
        Vector3 backPos = transform.position - (forwardVec * 10.0f);
        Quaternion flatRotation = Quaternion.LookRotation(forwardVec);
        ResetVehicle(backPos, flatRotation);
        yield return new WaitForSeconds(1f);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.EndCollisionEffect();
        }
        if (inputManager != null) inputManager.SetInputActive(true);
        _isRespawning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isRespawning) return;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.StartCollisionEffect();
        }
        StartCoroutine(CollisionRespawnRoutine());
    }
}