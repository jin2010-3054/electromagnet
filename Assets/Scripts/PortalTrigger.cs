using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public Transform portalA;
    public Transform portalB;
    public float cooldown = 0.2f; // 無限ループ防止
    private float lastTeleportTime = -1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time - lastTeleportTime > cooldown)
        {
            Teleport(other.transform);
            lastTeleportTime = Time.time;
        }
    }

    void Teleport(Transform player)
    {
        // 位置変換
        Vector3 localPos = portalA.InverseTransformPoint(player.position);
        localPos = Quaternion.Euler(0, 180f, 0) * localPos;
        player.position = portalB.TransformPoint(localPos);

        // 向き変換
        Quaternion localRot = Quaternion.Inverse(portalA.rotation) * player.rotation;
        localRot = Quaternion.Euler(0, 180f, 0) * localRot;
        player.rotation = portalB.rotation * localRot;

        // Rigidbody がある場合は速度も変換
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 localVel = portalA.InverseTransformDirection(rb.velocity);
            localVel = Quaternion.Euler(0, 180f, 0) * localVel;
            rb.velocity = portalB.TransformDirection(localVel);
        }
    }
}
