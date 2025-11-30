using UnityEngine;

public class Signal : MonoBehaviour
{
    [Header("ターゲット（向かせたい方向）")]
    public Transform target;

    [Header("ズーム設定")]
    public float zoomFOV = 30f;     // ズーム時の視野角
    public float zoomSpeed = 2f;    // ズーム速度

    [Header("向き補正")]
    public float lookSpeed = 3f;    // ターゲットを向く速さ

    private Camera mainCam;
    private float originalFOV;
    private bool isInside = false;

    void Start()
    {
        mainCam = Camera.main;
        originalFOV = mainCam.fieldOfView;
    }

    void Update()
    {
        if (isInside && target != null)
        {
            // --- ターゲットの方向を向く ---
            Quaternion lookRot = Quaternion.LookRotation(target.position - mainCam.transform.position);
            mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, lookRot, Time.deltaTime * lookSpeed);

            // --- FOVをズーム方向へ補間 ---
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            // --- FOVを元に戻す ---
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, originalFOV, Time.deltaTime * zoomSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isInside = false;
    }
}
