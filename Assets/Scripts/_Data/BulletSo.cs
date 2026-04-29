using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet Data", menuName = "Bullet")]
public class BullletSo : ScriptableObject
{
    [Header("기본 정보")]
    public float radius;
}