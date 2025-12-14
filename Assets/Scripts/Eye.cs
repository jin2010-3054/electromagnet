using UnityEngine;

public class TurnBottomToTarget : MonoBehaviour
{
    [Header("ターゲット設定")]
    public Transform target;  // 向けたい対象（磁石など）

    [Header("回転速度")]
    public float rotateSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // 対象の方向ベクトル
        Vector3 dir = target.position - transform.position;

        // 今の「下方向」と、向けたい方向
        Vector3 currentDown = transform.up * -1f;
        Vector3 desiredDown = dir.normalized;

        // 回転補間
        Quaternion currentRot = transform.rotation;

        // 今の"下"が desiredDown になるように計算
        Quaternion targetRot = Quaternion.FromToRotation(currentDown, desiredDown) * currentRot;

        transform.rotation = Quaternion.Slerp(
            currentRot,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
}
