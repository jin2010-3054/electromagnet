using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player1and2 : MonoBehaviour
{
    [Header("キャンバス設定")]
    public GameObject text;
    public GameObject image;

    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float downForce = 5f;
    public float maxSpeed = 10f;
    public Transform cam;

    [Header("ダッシュ設定")]
    public float dashSpeed = 12f;

    [Header("ジャンプ設定")]
    public int maxJumpCount = 2;
    private int jumpCount = 0;

    [Header("磁力設定")]
    public float magnetRange = 10f;

    Rigidbody rb;
    Renderer rend;

    public enum MagnetMode { None, North, South }
    public MagnetMode magnetMode = MagnetMode.None;

    [Header("色設定")]
    public Color northColor = Color.blue;
    public Color southColor = Color.red;

    private Color originalColor;
    private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();

        if (rend != null)
            originalColor = rend.material.color;

        UpdateColor();
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        /*
        if (rend.material.color == originalColor)
        {
            MoveProcess();
        }
        */
        //rb.velocity = new Vector3(0,0,0);


        ApplyMagneticForce();
        MoveProcess();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetMagnetMode(MagnetMode.None);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetMagnetMode(MagnetMode.North);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetMagnetMode(MagnetMode.South);

        // CキーでキャンバスON/OFF
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (text != null) text.SetActive(!text.activeSelf);
            if (image != null) image.SetActive(!image.activeSelf);
        }
    }

    void SetMagnetMode(MagnetMode mode)
    {
        magnetMode = mode;
        UpdateColor();
        Debug.Log("Magnet Mode: " + magnetMode);
    }

    void UpdateColor()
    {
        if (rend == null) return;

        switch (magnetMode)
        {
            case MagnetMode.None:
                rend.material.color = originalColor;
                break;
            case MagnetMode.North:
                rend.material.color = northColor;
                break;
            case MagnetMode.South:
                rend.material.color = southColor;
                break;
        }
    }

    void MoveProcess()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = dashSpeed;

        Vector3 vel = rb.velocity;
        vel.x = moveDir.x * currentSpeed+ vel.x;
        vel.z = moveDir.z * currentSpeed+ vel.z;

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            vel.y = jumpForce;
            jumpCount++;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            vel.y = -downForce;
        }


        if (vel.x > maxSpeed)
        {
            moveSpeed = maxSpeed;
        }

        rb.velocity = vel;
    }

    void ApplyMagneticForce()
    {
        if (magnetMode == MagnetMode.None) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, magnetRange);
        NoMoveMag strongestMag = null;
        float strongestValue = 0f;

        foreach (Collider hit in hits)
        {
            NoMoveMag mag = hit.GetComponent<NoMoveMag>();
            if (mag == null || mag.magnetPole == NoMoveMag.MagnetPole.None) continue;

            Vector3 closest = hit.ClosestPoint(transform.position);
            float dist = Vector3.Distance(transform.position, closest);

            if (dist > mag.range) continue;

            float intensity = mag.forcePower / (dist * dist + 0.1f); // 距離逆2乗
            if (intensity > strongestValue)
            {
                strongestValue = intensity;
                strongestMag = mag;
            }
        }

        if (strongestMag == null) return;

        Vector3 targetPos = strongestMag.GetComponent<Collider>().ClosestPoint(transform.position);
        Vector3 dir = (targetPos - transform.position).normalized;

        bool samePole =
            (magnetMode == MagnetMode.North && strongestMag.magnetPole == NoMoveMag.MagnetPole.North) ||
            (magnetMode == MagnetMode.South && strongestMag.magnetPole == NoMoveMag.MagnetPole.South);
        /*
        if (samePole)
            rb.AddForce(-dir * 50f, ForceMode.Acceleration);
        else
            rb.AddForce(dir * 50f, ForceMode.Acceleration);
        */
        if (samePole)
            rb.AddForce(-dir * strongestMag.forcePower, ForceMode.Acceleration);
        else
            rb.AddForce(dir * strongestMag.forcePower, ForceMode.Acceleration);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("NoMoveMagN") ||
            collision.gameObject.CompareTag("NoMoveMagS"))
            jumpCount = 0;

        if (collision.gameObject.CompareTag("Goal"))
        {
            if (text != null) text.SetActive(true);
            if (image != null) image.SetActive(true);

            Debug.Log("ゴールした！");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("NoMoveMagN") ||
            collision.gameObject.CompareTag("NoMoveMagS"))
            jumpCount = 0;
    }
}
