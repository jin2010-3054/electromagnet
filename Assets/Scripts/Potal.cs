using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("リンク")]
    public Portal linkedPortal; // ペア
    [Header("参照")]
    public Camera portalCamera; // 子に持たせるカメラ
    public MeshRenderer screen; // ポータル面に貼るRenderer

    [Header("設定")]
    public RenderTexture renderTexture;

    void Start()
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(1024, 1024, 16);
            renderTexture.Create();
        }
        portalCamera.targetTexture = renderTexture;
        screen.material.SetTexture("_MainTex", renderTexture);
    }

    void LateUpdate()
    {
        UpdateCameraToShowLinkedPortal();
    }

    void UpdateCameraToShowLinkedPortal()
    {
        Camera playerCam = Camera.main;
        if (playerCam == null || linkedPortal == null) return;

        // プレイヤーカメラのワールド位置 / 回転
        Transform playerT = playerCam.transform;

        // プレイヤーの位置を linkedPortal ローカルに変換
        Vector3 localPos = linkedPortal.transform.InverseTransformPoint(playerT.position);
        Vector3 mirroredPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
        Vector3 camWorldPos = transform.TransformPoint(mirroredPos);

        // 回転のマッピング
        Quaternion localRot = Quaternion.Inverse(linkedPortal.transform.rotation) * playerT.rotation;
        Quaternion camWorldRot = transform.rotation * localRot;

        portalCamera.transform.position = camWorldPos;
        portalCamera.transform.rotation = camWorldRot;

        // クリップ平面（オプション）でポータル背後の描画を切る場合の追加処理あり
    }

    // テレポート用（別スクリプトで OnTriggerEnter から呼ぶ想定）
    public void Teleport(Transform obj)
    {
        Vector3 localPos = transform.InverseTransformPoint(obj.position);
        Vector3 mappedPos = linkedPortal.transform.TransformPoint(new Vector3(-localPos.x, localPos.y, -localPos.z));

        Quaternion localRot = Quaternion.Inverse(transform.rotation) * obj.rotation;
        Quaternion mappedRot = linkedPortal.transform.rotation * localRot;

        obj.position = mappedPos;
        obj.rotation = mappedRot;
    }
}
