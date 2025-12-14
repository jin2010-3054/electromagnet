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
    public float shakeSpeed = 1f;

    [Header("ズーム設定（ホイールドラッグ）")]
    public float zoomSensitivity = 1f;
    public float minDistance = 2f;
    public float maxDistance = 12f;

    [Header("カメラ衝突（すり抜け防止）")]
    public float collideOffset = 0.2f;
    public LayerMask collisionMask;

    [Header("ダッシュ注目ターゲット")]
    public Transform dashLookTarget;     // ←ここで設定！
    public float dashLookSpeed = 6f;     // ターゲット方向へ向く速さ
    public bool isDashing = false;       // 外部から切り替え（Shiftなど）

    float zoomDistance = 6f;

    Rigidbody playerRb;
    Vector3 currentVelocity;
    Vector3 currentOffset;

    float pitch;
    float yaw;
    bool isMouseHeld = false;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (player != null)
            playerRb = player.GetComponent<Rigidbody>();

        currentOffset = groundOffset;

        Vector3 euler = transform.rotation.eulerAngles;
        pitch = euler.x;
        yaw = euler.y;
    }

    private void FixedUpdate()
    {
        // ダッシュ入力判定
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isDashing = true;
        }
        else
        {
            isDashing = false;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        bool isInAir = playerRb != null && !IsGrounded();

        // --- カメラ角度補間 ---
        float targetPitchOffset = isInAir ? airPitch : groundPitch;
        pitch = Mathf.Lerp(pitch, targetPitchOffset, Time.deltaTime * pitchSmooth);

        // --- オフセット補間 ---
        Vector3 targetOffset = isInAir ? airOffset : groundOffset;
        targetOffset.z = -zoomDistance;
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * moveSmooth);

        // --- ズーム ---
        if (Input.GetMouseButton(2))
        {
            float mouseY = Input.GetAxis("Mouse Y");
            zoomDistance += mouseY * zoomSensitivity * -1f;
            zoomDistance = Mathf.Clamp(zoomDistance, minDistance, maxDistance);
        }

        // --- 視点操作 ---
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

        Vector3 targetPos = player.position + Quaternion.Euler(0, yaw, 0) * currentOffset;

        // --- 衝突チェック ---
        RaycastHit hit;
        Vector3 dir = (targetPos - player.position).normalized;
        float distance = currentOffset.magnitude;

        if (Physics.Raycast(player.position, dir, out hit, distance, collisionMask))
        {
            float safeDist = hit.distance - collideOffset;
            safeDist = Mathf.Clamp(safeDist, minDistance, distance);
            targetPos = player.position + dir * safeDist;
        }

        Vector3 dirToCamera = (targetPos - player.position).normalized;
        float distToCamera = Vector3.Distance(player.position, targetPos);

        if (Physics.SphereCast(player.position, 0.3f, dirToCamera, out hit, distToCamera))
        {
            float safeDist = hit.distance - 0.1f;
            safeDist = Mathf.Max(0.5f, safeDist);
            targetPos = player.position + dirToCamera * safeDist;
        }

        // --- スムーズ追従 ---
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, 1f / moveSmooth);

        // --- ★ダッシュ中の自動注目カメラ ---
        if (isDashing && dashLookTarget != null && !isMouseHeld)
        {
            Vector3 lookDir = (dashLookTarget.position - transform.position).normalized;
            Quaternion targetLookRot = Quaternion.LookRotation(lookDir);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetLookRot,
                Time.deltaTime * dashLookSpeed
            );

            ApplyShake();
            return;
        }

        // --- 通常回転 ---
        Quaternion normalRot = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, normalRot, Time.deltaTime * rotateSmooth);

        ApplyShake();
    }

    void ApplyShake()
    {
        float shakeX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f * shakeAmount;
        float shakeY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f * shakeAmount;
        transform.position += new Vector3(shakeX, shakeY, 0f);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(player.position, Vector3.down, 1.1f);
    }
}
