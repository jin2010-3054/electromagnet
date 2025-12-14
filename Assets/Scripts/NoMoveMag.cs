using UnityEngine;

public class NoMoveMag : MonoBehaviour
{
    public enum MagnetPole { None, North, South }
    public enum MagnetType { Normal, AlwaysAttract, AlwaysRepel }

    [Header("¥—Íİ’è")]
    public MagnetPole magnetPole = MagnetPole.North;
    public MagnetType magnetType = MagnetType.Normal;

    public float forcePower = 40f; // Šî–{¥—Í
    public float range = 10f;      // ‰e‹¿”ÍˆÍ

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
