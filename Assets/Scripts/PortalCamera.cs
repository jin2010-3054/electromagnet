using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform portalA;
    public Transform portalB;
    public Transform player;

    void LateUpdate()
    {
        if (player == null || portalA == null || portalB == null) return;

        // プレイヤー位置をPortalA空間に変換
        Vector3 localPos = portalA.InverseTransformPoint(player.position);
        localPos = Quaternion.Euler(0, 180f, 0) * localPos;
        transform.position = portalB.TransformPoint(localPos);

        // プレイヤー向きも変換
        Quaternion localRot = Quaternion.Inverse(portalA.rotation) * player.rotation;
        localRot = Quaternion.Euler(0, 180f, 0) * localRot;
        transform.rotation = portalB.rotation * localRot;
    }
}
