using UnityEngine;

public class MagneticPull : MonoBehaviour
{
    [Header("磁石設定")]
    public Transform magnet;    // 磁石オブジェクト
    public float pullForce = 10f; // 引き寄せる力の強さ
    public float pullRange = 5f;  // 効果範囲

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 direction = magnet.position - transform.position; // 磁石への方向
        float distance = direction.magnitude;

        if (distance < pullRange) // 範囲内なら引き寄せ
        {
            direction.Normalize();
            float force = pullForce * (1 - distance / pullRange); // 距離が近いほど強く
            rb.AddForce(direction * force, ForceMode.Force);
        }
    }
}
