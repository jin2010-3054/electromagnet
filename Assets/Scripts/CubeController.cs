using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : MonoBehaviour
{
    [Header("動く速さWASD、下方向")] 
    public float moveSpeed = 5f;
    public float downForce = 5f;

    public Transform cam;

    [Header("最大ジャンプ回数-１")]
    public float jumpForce = 5f;
    public int maxJumpCount = 2; // 最大ジャンプ回数（例：二段ジャンプ）
    private int jumpCount = 0;   // 現在のジャンプ回数

    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 入力取得
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // カメラ基準で移動
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        Vector3 move = (camForward * v + camRight * h).normalized * moveSpeed;

        float currentY = rb.velocity.y;

        // ジャンプ（ジャンプ回数が最大未満のときのみ）
        if (Input.GetKey(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            currentY = jumpForce;
            jumpCount++;
        }

        // 下方向移動
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentY = -downForce;
        }

        rb.velocity = new Vector3(move.x, currentY, move.z);
    }

    // 地面に着いたらジャンプ回数リセット
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }
}
