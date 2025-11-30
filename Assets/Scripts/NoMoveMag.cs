using UnityEngine;

public class NoMoveMag : MonoBehaviour
{
    public enum MagnetPole { None, North, South }
    public MagnetPole magnetPole = MagnetPole.North;

    [Header("¥—Íİ’è")]
    public float forcePower = 40f; // Šî–{¥—Í
    public float range = 10f;      // ‰e‹¿”ÍˆÍ
}
