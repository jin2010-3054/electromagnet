using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("ターゲット設定")]
    public Transform player;

    [Header("追従スピード")]
    public float moveSmooth = 5f;
    public float rotateSmooth = 5f;

    [Header("オフセット（地上時）")]
    public Vector3 groundOffset = new Vector3(0f, 2f, -10f);

    [Header("オフセット（空中時・より上から）")]
    public Vector3 airOffset = new Vector3(0f, 8f, -10f);

    [Header("カメラ角度設定")]
    public float groundPitch = 10f;
    public float airPitch = 25f;
    public float pitchSmooth = 3f;

    [Header("マウス操作設定")]
    public float mouseSensitivity = 2f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("揺れ設定")]
    public float shakeAmount = 0.03f;
    public float shakeSpeed = 2f;

    [Header("ズーム設定（ホイールドラッグ）")]
    public float zoomSensitivity = 1f;
    public float minDistance = 2f;
    public float maxDistance = 12f;

    float zoomDistance = 6f; // 現在の距離（groundOffset.z の初期値に合わせる）
    

    Rigidbody playerRb;
    Vector3 currentVelocity;
    Vector3 currentOffset;

    float pitch;  // 上下角
    float yaw;    // 左右角
    bool isMouseHeld = false;

    void Start()
    {
        Cursor.visible = false;            // カーソルを消す
        Cursor.lockState = CursorLockMode.Locked; // 画面中央に固定（FPS/TPS風）

        if (player != null)
            playerRb = player.GetComponent<Rigidbody>();

        currentOffset = groundOffset;

        // 初期角度を設定
        Vector3 euler = transform.rotation.eulerAngles;
        pitch = euler.x;
        yaw = euler.y;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // --- 空中か地上かを判定 ---
        bool isInAir = playerRb != null && !IsGrounded();

        // --- カメラ角度補間 ---
        float targetPitchOffset = isInAir ? airPitch : groundPitch;
        pitch = Mathf.Lerp(pitch, targetPitchOffset, Time.deltaTime * pitchSmooth);

        // --- オフセット補間 ---
        Vector3 targetOffset = isInAir ? airOffset : groundOffset;
        targetOffset.z = -zoomDistance;
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * moveSmooth);

        // --- 中ボタン押しドラッグで距離変更 ---
        if (Input.GetMouseButton(2))
        {
            float mouseY = Input.GetAxis("Mouse Y");
            zoomDistance += mouseY * zoomSensitivity * -1f; // 上ドラッグで手前、下で遠ざかる

            zoomDistance = Mathf.Clamp(zoomDistance, minDistance, maxDistance);
        }

        // --- マウスクリック中だけ視点操作 ---
        if (Input.GetMouseButtonDown(1)) isMouseHeld = true;
        if (Input.GetMouseButtonUp(1)) isMouseHeld = false;

        if (isMouseHeld)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // --- 目標位置 ---
        Vector3 targetPos = player.position + Quaternion.Euler(0, yaw, 0) * currentOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, 1f / moveSmooth);

        // --- 目標回転 ---
        Quaternion targetRot = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSmooth);

        // --- 微揺れ ---
        float shakeX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f * shakeAmount;
        float shakeY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f * shakeAmount;
        transform.position += new Vector3(shakeX, shakeY, 0f);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(player.position, Vector3.down, 1.1f);
    }
}
